using System.Runtime.InteropServices;
using System;

using static nTrans2Quik.Utils;

namespace nTrans2Quik
{
    public partial class Trans2Quik
    {
        //Нечего тут смотреть, проходи дальше.
        //Сделано на базе trans2quik_api.h
        public const string Lib = "TRANS2QUIK.DLL";

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
        private static extern Int32 ExtСonnectToTerminal(string lpcstrConnectionParamsString, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Разрывает подключение к терминалу Quik.
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_DISCONNECT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtDisconnectFromTerminal(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Назначет делегат, который будет вызван извне при изменении статуса подключения к Терминалу
        /// </summary>
        /// <param name="pfConnectionStatusCallback">(Вход) Делегат</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtSetConnectionStatusCallBack(ExtConnStatusCallBack pfConnectionStatusCallback, Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Проверяет статус подключения к Терминалу
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns>Код ошибки</returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_IS_DLL_CONNECTED", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtIsDllConnected(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Делегат, который вызывается при подключении\отключении от\к Терминалу
        /// </summary>
        /// <param name="nConnectionEvent">10-Подключение установлено, 11-Подключение разорвано</param>
        /// <param name="nExtendedErrorCode">Код ошибки ОС</param>
        /// <param name="lpstrInfoMessage">Сообщение</param>
        private delegate void ExtConnStatusCallBack(Int32 nConnectionEvent, UInt32 nExtendedErrorCode, byte[] lpstrInfoMessage);

        /// <summary>
        /// Проверяет подключён ли теоминал к серверу Quik
        /// </summary>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход) Буфер для сообщения</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера сообщения</param>
        /// <returns>Код ошибки</returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_IS_QUIK_CONNECTED", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        static extern Int32 ExtIsQuikConnected(ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

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
        static extern Int32 ExtSendTransaction([MarshalAs(UnmanagedType.LPStr)] string lpstTransactionString, ref Int32 pnReplyCode, ref UInt32 pdwTransId, ref UInt64 pnOrderNum, byte[] lpstrResultMessage, UInt32 dwResultMessageSize, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);


        /// <summary>
        /// Отправляет транзакцию в асинхронном режиме. После завершения обработки библиотека дернет делегата заданного в методе ExtSendTransactionSetCallback
        /// </summary>
        /// <param name="transactionString">(Вход) Текст транзакции</param>
        /// <param name="nExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход\Выход) Буфер для сообщения об ошибке терминала</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера для сообщения об ошибке терминала</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SEND_ASYNC_TRANSACTION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtSendTransactionAsync([MarshalAs(UnmanagedType.LPStr)] string transactionString, ref Int32 nExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        /// <summary>
        /// Задаёт делегата, который будет вызываться после обработки асинхронных транзакций
        /// </summary>
        /// <param name="pfTransactionReplyCallback">Делегат</param>
        /// <param name="pnExtendedErrorCode">(Выход) Код ошибки ОС</param>
        /// <param name="lpstrErrorMessage">(Вход\Выход) Буфер для сообщения об ошибке терминала</param>
        /// <param name="dwErrorMessageSize">(Вход) Длинна буфера для сообщения об ошибке терминала</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtSendTransactionSetCallback(ExtAsyncTransactionResultDelegate pfTransactionReplyCallback, ref Int32 pnExtendedErrorCode, byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);

        private delegate void ExtAsyncTransactionResultDelegate(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode, Int32 nTransactionReplyCode, UInt32 dwTransId, UInt64 dOrderNum, [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage, IntPtr pTransReplyDescriptor);

        #endregion


        #region TransactionParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_CLASS_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranClassCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_SEC_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranSecCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_PRICE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTranPrice(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_QUANTITY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 ExtTranQty(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_BALANCE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int64 ExtTranBalance(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranFirmId(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranAccount(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranClientCode(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranBrokerRef(IntPtr tradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRANSACTION_REPLY_EXCHANGE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTranExchange(IntPtr tradeDescriptor);

        #endregion

        #region Orders

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SUBSCRIBE_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtSubscribeOrders([MarshalAs(UnmanagedType.LPStr)] string class_code, [MarshalAs(UnmanagedType.LPStr)] string sec_code);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtUnsubscribeOrders();

        private delegate void ExtOrderStatusCallback(Int32 nMode, Int32 dwTransID, UInt64 nOrderNum, [MarshalAs(UnmanagedType.LPStr)] string ClassCode, [MarshalAs(UnmanagedType.LPStr)] string SecCode, double dPrice, Int64 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, IntPtr pOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_START_ORDERS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtStartOrders(ExtOrderStatusCallback pfOrderStatusCallback);

        #endregion

        #region OrderParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int64 ExtOrdQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdDate(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_DATE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdDateTime(IntPtr nOrderDescriptor, Int32 nTimeType);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACTIVATION_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdActivationTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_WITHDRAW_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdWithdrawTime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXPIRY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdExpiry(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACCRUED_INT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern double ExtOrdAccuredInt(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_YIELD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern double ExtOrdYield(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_UID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdUid(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_VISIBLE_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int64 ExtOrdVisibleQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_PERIOD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdPeriod(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern System.Runtime.InteropServices.ComTypes.FILETIME ExtOrdFiletime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_WITHDRAW_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern System.Runtime.InteropServices.ComTypes.FILETIME ExtOrdWithdrawFiletime(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_USERID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdUserId(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdAccount(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdBrokerRef(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdClientCode(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdFirmId(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_VALUE_ENTRY_TYPE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdValueEntryType(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXTENDED_FLAGS", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdExtendedFlags(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_MIN_QTY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int64 ExtOrdMinQty(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_EXEC_TYPE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern Int32 ExtOrdExecType(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_AWG_PRICE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern double ExtOrdAWGPrice(IntPtr nOrderDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_ORDER_REJECT_REASON", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        private static extern string ExtOrdRejectReason(IntPtr nOrderDescriptor);

        #endregion

        #region Trades

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_SUBSCRIBE_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtSubscribeTrades([MarshalAs(UnmanagedType.LPStr)] string class_code, [MarshalAs(UnmanagedType.LPStr)] string sec_code);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtUnsubscribeTrades();

        public delegate void ExtTradeStatusCallback(Int32 nMode, UInt64 nNumber, UInt64 nOrderNumber, [MarshalAs(UnmanagedType.LPStr)] string ClassCode, [MarshalAs(UnmanagedType.LPStr)] string SecCode, Double dPrice, Int64 nQty, Double dValue, Int32 nIsSell, IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_START_TRADES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtStartTrades(ExtTradeStatusCallback pfTradeStatusCallback);

        #endregion

        #region TradeParameters

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdDate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_DATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdSettleDate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdTime(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_IS_MARGINAL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdIsMarginal(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdAccruedInt(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_YIELD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdYield(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TS_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdTsCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdClearingCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdExchangeCommisssion(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdTradingSystemCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PRICE2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdPrice2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_RATE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoRate(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_VALUE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoValue(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO2_VALUE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoValue2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdAccuedInt2(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_REPO_TERM", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdRepoTerm(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_START_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoStarDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_LOWER_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoLowerDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_UPPER_DISCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdRepoUpperDiscount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BLOCK_SECURITIES", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdBlockSecurities(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PERIOD", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdPeriod(IntPtr pTradeDescriptor);

        /// <summary>
        /// Получает одну из частей даты\времени сделки
        /// </summary>
        /// <param name="pTradeDescriptor"></param>
        /// <param name="nTimeType">0 - Дата, 1 - Время, 2 - Микросекунды</param>
        /// <returns></returns>
        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_DATE_TIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdDateTime(IntPtr pTradeDescriptor, Int32 nTimeType);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_FILETIME", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern System.Runtime.InteropServices.ComTypes.FILETIME ExtTrdFileTime(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_KIND", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdKind(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CURRENCY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdCcy(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CURRENCY", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdSettleCcy(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdSettleCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_ACCOUNT", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdAccount(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BROKERREF", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdBrokerRef(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_CLIENT_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdClientCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_USERID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdUserId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdFirmId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_PARTNER_FIRMID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdPartnerFirmId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_CODE", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdExchangeCode(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_STATION_ID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(QuikStrMarshaler))]
        public static extern string ExtTrdTradeStationId(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_BROKER_COMMISSION", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double ExtTrdBrokerCommission(IntPtr pTradeDescriptor);

        [DllImport(Lib, EntryPoint = "TRANS2QUIK_TRADE_TRANSID", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern Int32 ExtTrdTranSID(IntPtr pTradeDescriptor);

        #endregion



    }
}
