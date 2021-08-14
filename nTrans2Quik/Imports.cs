using System.Runtime.InteropServices;
using System;

using static nTrans2Quik.Utils;

namespace nTrans2Quik
{
    internal static class Ext
    {
        //Нечего тут смотреть, проходи дальше.
        //Сделано на базе trans2quik_api.h
        public const string Lib = "TRANS2QUIK.DLL";


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lib);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void FreeLibrary(IntPtr module);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr module, string proc);

        public static IntPtr Trans2QuikPtr;


        #region Connections

        /// <summary>
        ///  Устанавливает подключение к терминалу Quik.
        ///  Требует вкоючённой функуции в терминале "обработка внешних транзакций"
        /// </summary>
        /// <param name="lpcstrConnectionParamsString">(Вход) Путь к работающему терминалу Quik</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_CONNECT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 СonnectToTerminal(string lpcstrConnectionParamsString, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Разрывает подключение к терминалу Quik.
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_DISCONNECT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 DisconnectFromTerminal(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Назначет делегат, который будет вызван извне при изменении статуса подключения к Терминалу
        /// </summary>
        /// <param name="pfConnectionStatusCallback">(Вход) Делегат</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SetConnectionStatusCallBack(ConnStatusCallBack pfConnectionStatusCallback, Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Проверяет статус подключения к Терминалу
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns>Код ошибки</returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_IS_DLL_CONNECTED", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 IsDllConnected(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Делегат, который вызывается при подключении\отключении от\к Терминалу
        /// </summary>
        /// <param name="nConnectionEvent">10-Подключение установлено, 11-Подключение разорвано</param>
        /// <param name="nExtendedErrorCode">Код ошибки ОС</param>
        /// <param name="lpstrInfoMessage">Сообщение</param>
        public delegate void ConnStatusCallBack(Int32 nConnectionEvent, UInt32 nExtendedErrorCode, byte[] lpstrInfoMessage);

        /// <summary>
        /// Проверяет подключён ли теоминал к серверу Quik
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns>Код ошибки</returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_IS_QUIK_CONNECTED", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 IsQuikConnected(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        #endregion


        #region Transactions

        /// <summary>
        /// Отправляет транзакцию в Quik
        /// </summary>
        /// <param name="lpstTransactionString">(Вход) Текст транзакции</param>
        /// <param name="pnReplyCode">(Выход) код ответа</param>
        /// <param name="pdwTransId">(Выход) ID транзакции</param>
        /// <param name="pnOrderNum">(Выход) Номер ордера</param>
        /// <param name="lpstrResultMessage">(Вход\Выход) Буфер для сообщения терминала</param>
        /// <param name="dwResultMessageSize">(Вход) Длинна буфера для сообщения терминала</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход\Выход) Буфер для сообщения об ошибке терминала</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера для сообщения об ошибке терминала</param>
        /// <returns>Код ошибки</returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SEND_SYNC_TRANSACTION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SendTransaction([MarshalAs(UnmanagedType.LPStr)] string lpstTransactionString, ref Int32 pnReplyCode, ref UInt32 pdwTransId, ref UInt64 pnOrderNum, byte[] lpstrResultMessage, UInt32 dwResultMessageSize, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);


        /// <summary>
        /// Отправляет транзакцию в асинхронном режиме. После завершения обработки библиотека дернет делегата заданного в методе SendTransactionSetCallback
        /// </summary>
        /// <param name="transactionString">(Вход) Текст транзакции</param>
        /// <param name="nExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход\Выход) Буфер для сообщения об ошибке терминала</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера для сообщения об ошибке терминала</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SEND_ASYNC_TRANSACTION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SendTransactionAsync([MarshalAs(UnmanagedType.LPStr)] string transactionString, ref Int32 nExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Задаёт делегата, который будет вызываться после обработки асинхронных транзакций
        /// </summary>
        /// <param name="pfTransactionReplyCallback">Делегат</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход\Выход) Буфер для сообщения об ошибке терминала</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера для сообщения об ошибке терминала</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SendTransactionSetCallback(AsyncTransactionResultDelegate pfTransactionReplyCallback, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        public delegate void AsyncTransactionResultDelegate(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode, Int32 nTransactionReplyCode, UInt32 dwTransId, UInt64 dOrderNum, [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage, IntPtr pTransReplyDescriptor);

        #endregion


        #region TransactionParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_CLASS_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranClassCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_SEC_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranSecCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_PRICE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TranPrice(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_QUANTITY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 TranQty(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_BALANCE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 TranBalance(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranFirmId(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranAccount(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranClientCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranBrokerRef(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_EXCHANGE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TranExchange(IntPtr tradeDescriptor);

        #endregion

        #region Orders

        /// <summary>
        /// Подписаться на событие новых ордеров.
        /// </summary>
        /// <param name="class_code">Код класса, если пустая строка то ВСЕ</param>
        /// <param name="sec_code">Код инструмента, если пусто то ВСЕ</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SUBSCRIBE_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SubscribeOrders([MarshalAs(UnmanagedType.LPStr)] string class_code, [MarshalAs(UnmanagedType.LPStr)] string sec_code);

        /// <summary>
        /// Отписаться от получение новых ордеров
        /// </summary>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 UnsubscribeOrders();

        public delegate void OrderStatusCallback(Int32 nMode, Int32 dwTransID, UInt64 nOrderNum, [MarshalAs(UnmanagedType.LPStr)] string ClassCode, [MarshalAs(UnmanagedType.LPStr)] string SecCode, double dPrice, Int64 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, IntPtr pOrderDescriptor);

        /// <summary>
        /// Начать получение ордеров. Все ранее созданные ордера будут переотправлены
        /// </summary>
        /// <param name="pfOrderStatusCallback"></param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_START_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 StartOrders(OrderStatusCallback pfOrderStatusCallback);

        #endregion

        #region OrderParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 OrdQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdDate(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_DATE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdDateTime(IntPtr nOrderDescriptor, Int32 nTimeType);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACTIVATION_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdActivationTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_WITHDRAW_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdWithdrawTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXPIRY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdExpiry(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACCRUED_INT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double OrdAccuredInt(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_YIELD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double OrdYield(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_UID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdUid(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_VISIBLE_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 OrdVisibleQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_PERIOD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdPeriod(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern System.Runtime.InteropServices.ComTypes.FILETIME OrdFiletime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_WITHDRAW_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern System.Runtime.InteropServices.ComTypes.FILETIME OrdWithdrawFiletime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_USERID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdUserId(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdAccount(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdBrokerRef(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdClientCode(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdFirmId(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_VALUE_ENTRY_TYPE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdValueEntryType(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXTENDED_FLAGS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdExtendedFlags(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_MIN_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 OrdMinQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXEC_TYPE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 OrdExecType(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_AWG_PRICE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double OrdAWGPrice(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_REJECT_REASON", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string OrdRejectReason(IntPtr nOrderDescriptor);

        #endregion

        #region Trades

        /// <summary>
        ///  Подписаться на событие новых сделок
        /// </summary>
        /// <param name="class_code">Код класса, если пустая строка то ВСЕ</param>
        /// <param name="sec_code">Код инструмента, если пусто то ВСЕ</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SUBSCRIBE_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 SubscribeTrades([MarshalAs(UnmanagedType.LPStr)] string class_code, [MarshalAs(UnmanagedType.LPStr)] string sec_code);

        /// <summary>
        /// Отписаться от получение новых сделок
        /// </summary>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 UnsubscribeTrades();

        public delegate void TradeStatusCallback(Int32 nMode, UInt64 nNumber, UInt64 nOrderNumber, [MarshalAs(UnmanagedType.LPStr)] string ClassCode, [MarshalAs(UnmanagedType.LPStr)] string SecCode, Double dPrice, Int64 nQty, Double dValue, Int32 nIsSell, IntPtr pTradeDescriptor);

        /// <summary>
        /// Начать получение сделок. Все ранее созданные сделки будут переотправлены
        /// </summary>
        /// <param name="pfTradeStatusCallback"></param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_START_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 StartTrades(TradeStatusCallback pfTradeStatusCallback);

        #endregion

        #region TradeParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdDate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdSettleDate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdTime(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_IS_MARGINAL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdIsMarginal(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdAccruedInt(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_YIELD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdYield(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TS_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdTsCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdClearingCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdExchangeCommisssion(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdTradingSystemCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PRICE2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdPrice2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_RATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoRate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_VALUE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoValue(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO2_VALUE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoValue2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdAccuedInt2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_TERM", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdRepoTerm(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_START_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoStarDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_LOWER_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoLowerDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_UPPER_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdRepoUpperDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BLOCK_SECURITIES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdBlockSecurities(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PERIOD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdPeriod(IntPtr pTradeDescriptor);

        /// <summary>
        /// Получает одну из частей даты\времени сделки
        /// </summary>
        /// <param name="pTradeDescriptor"></param>
        /// <param name="nTimeType">0 - Дата, 1 - Время, 2 - Микросекунды</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_DATE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdDateTime(IntPtr pTradeDescriptor, Int32 nTimeType);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern System.Runtime.InteropServices.ComTypes.FILETIME TrdFileTime(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_KIND", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdKind(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CURRENCY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdCcy(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CURRENCY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdSettleCcy(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdSettleCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdAccount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdBrokerRef(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdClientCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_USERID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdUserId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdFirmId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PARTNER_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdPartnerFirmId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdExchangeCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_STATION_ID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string TrdTradeStationId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BROKER_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double TrdBrokerCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TRANSID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 TrdTranSID(IntPtr pTradeDescriptor);

        #endregion



    }
}
