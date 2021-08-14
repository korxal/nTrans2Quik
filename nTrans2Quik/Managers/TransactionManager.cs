using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{
    public class TransactionManager
    {

        /// <summary>
        /// Получить транзакцию из кэша по номеру ордера
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public Transaction GetTransaction(uint TranId) => TransactionCache.ContainsKey(TranId) ? TransactionCache[TranId] : null;
        public ConcurrentDictionary<uint, Transaction> TransactionCache = new ConcurrentDictionary<uint, Transaction>();
        public delegate void TransactionDelegate(Transaction t);
        public event TransactionDelegate NewTransaction;

        //Ссылка на делегат. Иначе за делегатом придёт заботливый GC
        private Ext.AsyncTransactionResultDelegate AsyncTransactionResultDelegate;




        /// <summary>
        /// Привязывает к Unmanaged событию появления новой танзакции в терминале наш делегат (NewTransactionEvent)
        /// </summary>
        public void AttachTransactionEvent()
        {
            Int32 ErrorCode = 0;
            byte[] ErrorMessage = new byte[1024];
            AsyncTransactionResultDelegate = new Ext.AsyncTransactionResultDelegate(NewTransactionEvent);
            Ext.SendTransactionSetCallback(AsyncTransactionResultDelegate, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
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
            Transaction rez = new Transaction((int)dwTransId)
            {
                OrderNum = dOrderNum,
                Status = nTransactionResult,
                ClassCode = Ext.TranClassCode(pTransReplyDescriptor),
                SecCode = Ext.TranSecCode(pTransReplyDescriptor),
                Price = Ext.TranPrice(pTransReplyDescriptor),
                Qty = Ext.TranQty(pTransReplyDescriptor),
                Balance = Ext.TranBalance(pTransReplyDescriptor),
                Account = Ext.TranAccount(pTransReplyDescriptor),
                ClientCode = Ext.TranClientCode(pTransReplyDescriptor),
                ErrorMessage = TransactionReplyMessage,
                TerminalMessage = TransactionReplyMessage,
            };

            //Добавить в кэш
            if (!TransactionCache.ContainsKey(dwTransId)) TransactionCache.TryAdd(dwTransId, rez);
            NewTransaction?.Invoke(rez);

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

            var status = Ext.SendTransaction(Transaction, ref ReplyCode, ref TranId, ref OrderNum, Message, (UInt32)Message.Length, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);

            Transaction rez = new Transaction((int)TranId)
            {
                ErrorMessage = Utils.Str(ErrorMessage),
                OrderNum = OrderNum,
                TerminalMessage = Utils.Str(Message)
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

    }
}
