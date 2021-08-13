using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace nTrans2Quik
{
    public partial class Trans2Quik : IDisposable
    {

        private bool DllConnected;
        public delegate void ConnectionStateChangedDelegate(bool IsConnected);
        public event ConnectionStateChangedDelegate ConnectionStateChanged;

        public delegate void TransactionDelegate(Transaction t);
        public event TransactionDelegate NewTransaction;

        public delegate void OrderDelegate(Order o);
        public event OrderDelegate NewOrder;

        public delegate void TradeDelegate(Trade o);
        public event TradeDelegate NewTrade;

        public ConcurrentDictionary<ulong, Order> OrderCache = new ConcurrentDictionary<ulong, Order>();
        public ConcurrentDictionary<ulong, Trade> TradeCache = new ConcurrentDictionary<ulong, Trade>();
        public ConcurrentDictionary<ulong, Transaction> TransactionCache = new ConcurrentDictionary<ulong, Transaction>();


        public Transaction GetTransaction(ulong OrderNumber) => TransactionCache.ContainsKey(OrderNumber) ? TransactionCache[OrderNumber] : null;
        public Order GetOrder(ulong OrderNumber) => OrderCache.ContainsKey(OrderNumber) ? OrderCache[OrderNumber] : null;
        public Trade GetTrade(ulong TradeNumber) => TradeCache.ContainsKey(TradeNumber) ? TradeCache[TradeNumber] : null;

        public List<Trade> GetOrderTrades(ulong OrderNumber) => OrderCache.ContainsKey(OrderNumber) ? TradeCache.Values.Where(x => x.OrderNumber == OrderNumber).ToList(): null;


        private readonly Encoding RU = Encoding.GetEncoding(1251);
        private string Str(byte[] arr) => RU.GetString(arr).TrimEnd('\0');// Для чтения C строк


        //Ссылка на делегаты. Иначе за делегатом придёт заботливый GC
        private ExtAsyncTransactionResultDelegate AsyncTransactionResultDelegate;
        private ExtOrderStatusCallback AsyncOrderStatusCallback;
        private ExtTradeStatusCallback AsyncTradeStatusCallback;
        private ExtConnStatusCallBack AsyncConnStatusCallBack;


        /// <summary>
        /// Делегат статуса подключения к Терминалу. Вызывается библиотекой Trans2Quik когда меняется статус подключения.
        /// </summary>
        /// <param name="nConnectionEvent">Статус подключения</param>
        /// <param name="nExtendedErrorCode">Код ошибки (из ОС)</param>
        /// <param name="lpstrInfoMessage">Сообщение терминала</param>
        private void ConnectionStatusDelegate(Int32 nConnectionEvent, UInt32 nExtendedErrorCode, byte[] lpstrInfoMessage)
        {
            //10 = connected
            DllConnected = nConnectionEvent == 10;
            ConnectionStateChanged?.Invoke(nConnectionEvent == 10);
        }


        /// <summary>
        /// Метод вызывается библиотекой Trans2Quik при появлении новой транзакции
        /// </summary>
        /// <param name="nTransactionResult">Результат транзакции </param>
        /// <param name="nTransactionExtendedErrorCode"> Код ошибки ОС</param>
        /// <param name="nTransactionReplyCode">Код ответа</param>
        /// <param name="dwTransId">Внутренний идентификатор транзакции</param>
        /// <param name="dOrderNum">Номер заявки в торговой системе</param>
        /// <param name="TransactionReplyMessage">Сообщение терминала</param>
        /// <param name="pTransReplyDescriptor">Дескриптор транзакции. Нужен для получения деталей транзакции</param>
        private void NewTransactionEvent(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode, Int32 nTransactionReplyCode, UInt32 dwTransId, UInt64 dOrderNum, string TransactionReplyMessage, IntPtr pTransReplyDescriptor)
        {
            Transaction rez = new Transaction()
            {
                OrderNum = dOrderNum,
                Status = nTransactionResult,
                ClassCode = ExtTranClassCode(pTransReplyDescriptor),
                SecCode = ExtTranSecCode(pTransReplyDescriptor),
                Price = ExtTranPrice(pTransReplyDescriptor),
                Qty = ExtTranQty(pTransReplyDescriptor),
                Balance = ExtTranBalance(pTransReplyDescriptor),
                Account = ExtTranAccount(pTransReplyDescriptor),
                ClientCode = ExtTranClientCode(pTransReplyDescriptor),
                ErrorMessage = TransactionReplyMessage,
                InternalId = dwTransId,
                TerminalMessage = TransactionReplyMessage,
            };

            TransactionCache.TryAdd(dOrderNum, rez);
            NewTransaction?.Invoke(rez);

        }


        /// <summary>
        /// Метод вызывается библиотекой Trans2Quik при появлении новой заявки
        /// </summary>
        /// <param name="nMode">0 если новая заявка, не 0 если повтор.</param>
        /// <param name="dwTransID">Внутренний идентификатор транзакции</param>
        /// <param name="nOrderNum">Номер заявки в ТС</param>
        /// <param name="ClassCode">Код класса </param>
        /// <param name="SecCode">Код бумаги</param>
        /// <param name="dPrice">Цена</param>
        /// <param name="nBalance"></param>
        /// <param name="dValue"></param>
        /// <param name="nIsSell"></param>
        /// <param name="nStatus"></param>
        /// <param name="pOrderDescriptor">Дескриптор заявки. Нужен для получения деталей заявки</param>
        private void NewOrderEvent(Int32 nMode, Int32 dwTransID, UInt64 nOrderNum, string ClassCode, string SecCode, double dPrice, Int64 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, IntPtr pOrderDescriptor)
        {

            Order o = new Order()
            {
                IsNew = nMode == 0,
                TransactionId = dwTransID,
                OrderNum = nOrderNum,
                ClassCode = ClassCode,
                SecCode = SecCode,
                Price = dPrice,
                Balance = nBalance,
                Value = dValue,
                Side = nIsSell == 1 ? Side.Sell : Side.Buy,
                Status = nStatus,
            };

            o.Qty = ExtOrdQty(pOrderDescriptor);
            o.Yield = ExtOrdYield(pOrderDescriptor);
            o.VisibleQty = ExtOrdVisibleQty(pOrderDescriptor);
            o.ClientCode = ExtOrdClientCode(pOrderDescriptor);
            o.Account = ExtOrdAccount(pOrderDescriptor);
            o.AccruedInt = ExtOrdAccuredInt(pOrderDescriptor);
            o.AwgPrice = ExtOrdAWGPrice(pOrderDescriptor);
            o.BrokerRef = ExtOrdBrokerRef(pOrderDescriptor);
            o.ExecType = ExtOrdExecType(pOrderDescriptor);
            o.ExtendedFlags = ExtOrdExtendedFlags(pOrderDescriptor);
            o.MinQty = ExtOrdMinQty(pOrderDescriptor);
            o.RejectReason = ExtOrdRejectReason(pOrderDescriptor);
            o.UID = ExtOrdUid(pOrderDescriptor);
            o.UserId = ExtOrdUserId(pOrderDescriptor);
            o.Period = ExtOrdPeriod(pOrderDescriptor);
            o.ActivationDate = Utils.FromArqaTime(ExtOrdActivationTime(pOrderDescriptor));
            o.ExpiryDate = Utils.FromArqaTime(ExtOrdExpiry(pOrderDescriptor));
            o.WithdrawalDate = Utils.FromArqaTime(ExtOrdWithdrawTime(pOrderDescriptor));
            o.Date = Utils.FromArqaDealDate(ExtOrdDateTime(pOrderDescriptor, 0), ExtOrdDateTime(pOrderDescriptor, 1), ExtOrdDateTime(pOrderDescriptor, 2));

            OrderCache.TryAdd(o.OrderNum, o);

            NewOrder?.Invoke(o);

        }

        private void NewTradeEvent(Int32 nMode, UInt64 nNumber, UInt64 nOrderNumber, string ClassCode, string SecCode, Double dPrice, Int64 nQty, Double dValue, Int32 nIsSell, IntPtr pTradeDescriptor)
        {

            Trade t = new Trade()
            {
                Mode = nMode,
                Number = nNumber,
                OrderNumber = nOrderNumber,
                ClassCode = ClassCode,
                SecCode = SecCode,
                Price = dPrice,
                Qty = nQty,
                Value = dValue,
                Side = nIsSell == 1 ? Side.Sell : Side.Buy,

            };


            t.Account = ExtTrdAccount(pTradeDescriptor);
            t.AccruedInt = ExtTrdAccruedInt(pTradeDescriptor);
            t.AccruedInt2 = ExtTrdAccuedInt2(pTradeDescriptor);
            t.BlockSec = ExtTrdBlockSecurities(pTradeDescriptor);
            t.BrokerRef = ExtTrdBrokerRef(pTradeDescriptor);
            t.ClientCode = ExtTrdClientCode(pTradeDescriptor);
            t.Ccy = ExtTrdCcy(pTradeDescriptor);
            t.CommBroker = ExtTrdBrokerCommission(pTradeDescriptor);
            t.CommClearing = ExtTrdClearingCommission(pTradeDescriptor);
            t.CommExchange = ExtTrdExchangeCommisssion(pTradeDescriptor);
            t.CommTradingSys = ExtTrdTradingSystemCommission(pTradeDescriptor);
            t.CommTS = ExtTrdTsCommission(pTradeDescriptor);
            t.ExchangeCode = ExtTrdExchangeCode(pTradeDescriptor);
            t.FirmId = ExtTrdFirmId(pTradeDescriptor);
            t.IsMarginal = ExtTrdIsMarginal(pTradeDescriptor) == 1;
            t.Kind = ExtTrdKind(pTradeDescriptor);
            t.Price2 = ExtTrdPrice2(pTradeDescriptor);
            t.PartnerFirmId = ExtTrdPartnerFirmId(pTradeDescriptor);
            t.RepoDisc = ExtTrdRepoStarDiscount(pTradeDescriptor);
            t.RepoDiscHigh = ExtTrdRepoUpperDiscount(pTradeDescriptor);
            t.RepoDiscLow = ExtTrdRepoLowerDiscount(pTradeDescriptor);
            t.RepoRate = ExtTrdRepoRate(pTradeDescriptor);
            t.RepoTerm = ExtTrdRepoTerm(pTradeDescriptor);
            t.StationId = ExtTrdTradeStationId(pTradeDescriptor);
            t.UserId = ExtTrdUserId(pTradeDescriptor);
            t.Yield = ExtTrdYield(pTradeDescriptor);
            t.Volume1 = ExtTrdRepoValue(pTradeDescriptor);
            t.Volume2 = ExtTrdRepoValue2(pTradeDescriptor);
            TradeCache.TryAdd(t.Number, t);
            NewTrade?.Invoke(t);

        }

        /// <summary>
        /// Привязывает к Unmanaged событию появления новой танзакции в терминале наш делегат (NewTransactionEvent)
        /// </summary>
        private void AttachTransactionEvent()
        {
            Int32 ErrorCode = 0;
            byte[] ErrorMessage = new byte[1024];
            AsyncTransactionResultDelegate = new ExtAsyncTransactionResultDelegate(NewTransactionEvent);
            ExtSendTransactionSetCallback(AsyncTransactionResultDelegate, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
        }


        /// <summary>
        /// Привязывает к Unmanaged событию появления новой заявке в терминале наш делегат (NewOrderEvent)
        /// </summary>
        private void AttachOrderEvent()
        {
            AsyncOrderStatusCallback = new ExtOrderStatusCallback(NewOrderEvent);
            ExtSubscribeOrders("", "");//Подписка на все ордера
            ExtStartOrders(AsyncOrderStatusCallback);
        }

        /// <summary>
        /// Привязывает к Unmanaged событию появления новой сделки в терминале наш делегат (NewTradeEvent)
        /// </summary>
        private void AttachTradeEvent()
        {
            AsyncTradeStatusCallback = new ExtTradeStatusCallback(NewTradeEvent);
            ExtSubscribeTrades("", "");//Подписка на все сделки
            ExtStartTrades(AsyncTradeStatusCallback);
        }

        /// <summary>
        /// Устанавливает подключение к терминалу Quik 
        /// </summary>
        /// <param name="QuikPath">Путь к терминалу Quik</param>
        public void ConnectToQuikTerminal(string QuikPath)
        {
            byte[] ErrorMessage = new byte[50];
            Int32 ErrorCode = 0;
            Int32 rez;

            try
            {
                AsyncConnStatusCallBack = new ExtConnStatusCallBack(ConnectionStatusDelegate);
                rez = ExtSetConnectionStatusCallBack(AsyncConnStatusCallBack, ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            }
            catch (BadImageFormatException)
            {
                //Не cмогли подключить unmanaged библиоткеку, давайте разбираться.
                //if(Environment.Is64BitProcess && IntPtr.Size == 4) throw new Exception("А мы точно в Винде и на Intel?") 

                //Environment.Is64BitProcess для excel может не прокатить
                if (IntPtr.Size == 4)
                    //Иди сними кнопку "Prefer 32 Bits"
                    throw new Exception("Ошибка подключения библиотеки Trans2Quik. 32-битный С# модуль попытался загруть 64-битный Trans2Quik.dll");
                else
                    throw new Exception("Ошибка подключения библиотеки Trans2Quik. 64-битный С# модуль попытался загруть 32-битный Trans2Quik.dll");
            }

            //Пара проверок что путь до терминала валидный.
            if (!Directory.Exists(QuikPath)) throw new DirectoryNotFoundException("Путь не найден:" + QuikPath);
            if (!File.Exists(Path.Combine(QuikPath, "info.exe"))) throw new FileNotFoundException("Info.exe не найден в " + QuikPath);

            //Проверить что терминал Квик готов к подключению
            string QuikPipePathIncoming = "*WorkDir#" + QuikPath.TrimEnd('\\').Replace(":", "_").Replace("\\", "#") + "#QUIK_REQUESTS_INCOMING";
            string QuikPipePathOutgoing = "*WorkDir#" + QuikPath.TrimEnd('\\').Replace(":", "_").Replace("\\", "#") + "#QUIK_REQUESTS_OUTCOMING";

            if (Directory.GetFiles("\\\\.\\pipe\\", QuikPipePathIncoming).Length != 1 ||
                Directory.GetFiles("\\\\.\\pipe\\", QuikPipePathOutgoing).Length != 1)
                throw new Exception("Либо терминал не запущен, либо выключена обработка внешних транзакций. Сервисы -> Экспорт\\импорт данных -> внешние транзакции");


            rez = ExtСonnectToTerminal(QuikPath, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);

            //231 - ERROR_PIPE_BUSY - Кто-то уже подключен.
            if (ErrorCode == 231) throw new Exception($"Терминал Quik не отвечает, пожалуйста выключите и снова включите обработку внешних транзакий в терминале (Сервисы -> Экспорт\\импорт данных -> внешние транзакции)");
            //109 - ERROR_BROKEN_PIPE  - Терминал скорее всего уже мёртв к этому времени...
            if (ErrorCode == 109) throw new Exception($"Похоже что терминал Quik завис, пожалуйста перезапустите терминал");

            if (rez != 0)
                throw new Exception($"Ошибка подключения к терминалу Quik, код ошибки {ErrorCode}, описание ошибки:'{ErrorDecode(ErrorCode)}', Сообщение терминала:{Encoding.ASCII.GetString(ErrorMessage).TrimEnd((char)0)}");



            rez = ExtIsDllConnected(ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            //10 = Подключено к терминалу
            if (rez != 10) throw new Exception("Подключение не установлено");

            //Подцепить обработчик транзакций
            AttachTransactionEvent();

            //Подцепить обработчик ордеров
            AttachOrderEvent();

            //Подцепить обработчик сделок
            AttachTradeEvent();

            DllConnected = true;
        }


        /// <summary>
        /// Разрывает подключение к терминалу Quik
        /// </summary>
        public void DisconnectFromQuikTerminal()
        {
            byte[] ErrorMessage = new Byte[255];
            int ErrorCode = 0;
            var rez = ExtDisconnectFromTerminal(ref ErrorCode, ErrorMessage, (uint)ErrorMessage.Length);
            if (rez != 0) throw new Exception($"Не удалось отключиться от Терминала Quik, код ошибки {rez}, описание ошибки:'{ErrorDecode(rez)}'");
            DllConnected = false;
        }

        /// <summary>
        /// Проверяет статус подключения Терминала к вышестоящему серверу Quik
        /// </summary>
        /// <returns>Вернет true если подключён</returns>
        public bool IsQuikConnectedToServer()
        {
            byte[] ErrorMessage = new Byte[255];
            int ErrorCode = 0;
            var rez = ExtIsQuikConnected(ref ErrorCode, ErrorMessage, (uint)ErrorMessage.Length);
            if (ErrorCode == 0 && rez == 8) return true;
            return false;
        }

        /// <summary>
        /// Проверяет подключена ли библиотека Trans2Quik к Терминалу 
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToQuikTerminal()
        {
            byte[] ErrorMessage = new byte[1024];
            Int32 ErrorCode = 0;
            Int32 rez;
            rez = ExtIsDllConnected(ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            return rez == 10;
        }


        /// <summary>
        /// Отправляет транзакцию в Quik
        /// Если транзакция успешна, то будет заполнен OrderNum
        /// </summary>
        /// <param name="Transaction">Текст транзакции в формате QUIK</param>
        /// <returns>Заполненный класс транзакции</returns>
        public Transaction SendTransaction(string Transaction)
        {
            UInt64 OrderNum = 0;
            UInt32 TranId = 0;
            Int32 ErrorCode = 0;
            Int32 ReplyCode = 0;
            byte[] ErrorMessage = new Byte[1024];
            byte[] Message = new Byte[1024];

            var status = ExtSendTransaction(Transaction, ref ReplyCode, ref TranId, ref OrderNum, Message, (UInt32)Message.Length, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);

            Transaction rez = new Transaction()
            {
                ErrorMessage = Str(ErrorMessage),
                InternalId = TranId,
                OrderNum = OrderNum,
                TerminalMessage = Str(Message)
            };

            return rez;
        }




        public Transaction SendTransaction(Transaction t)
        {

            StringBuilder sb = new StringBuilder();


            sb.V("TRANS_ID", t.InternalId);

            switch (t.TransactionType)
            {
                case TranType.New:
                    sb.V("ACTION", "NEW_ORDER");
                    break;
                case TranType.Kill:
                    sb.V("ACTION", "KILL_ORDER");
                    break;
            }

            switch (t.Side)
            {
                case Side.Buy:
                    sb.V("OPERATION", "B");
                    break;
                case Side.Sell:
                    sb.V("OPERATION", "S");
                    break;
            }

            switch (t.ExecutionCondition)
            {
                case ExecCondiotion.FillOrKill:
                    sb.V("EXECUTION_CONDITION", "FILL_OR_KILL");
                    break;
                case ExecCondiotion.KillBalance:
                    sb.V("EXECUTION_CONDITION", "KILL_BALANCE");
                    break;
            }

            sb.V("CLASSCODE", t.ClassCode);
            sb.V("SECCODE", t.SecCode);
            sb.V("PRICE", t.Price);
            sb.V("ACCOUNT", t.Account);
            sb.V("QUANTITY", t.Qty);
            sb.V("CLIENT_CODE", t.ClientCode);
            sb.V("ORDER_KEY", t.OrderNum);
            sb.V("COMMENT", t.Comment);



            return SendTransaction(sb.ToString());
        }



        public void Dispose()
        {
            if (!DllConnected) return;
            try { ExtUnsubscribeOrders(); } catch { }
            try { ExtUnsubscribeTrades(); } catch { }
            try { DisconnectFromQuikTerminal(); } catch { }

        }
    }
}