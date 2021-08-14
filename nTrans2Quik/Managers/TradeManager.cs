using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{
    public class TradeManager
    {

        /// <summary>
        /// Получить сделку из кэша по номеру сделки
        /// </summary>
        /// <param name="TradeNumber"></param>
        /// <returns></returns>
        public Trade GetTrade(ulong TradeNumber) => TradeCache.ContainsKey(TradeNumber) ? TradeCache[TradeNumber] : null;
        public ConcurrentDictionary<ulong, Trade> TradeCache = new ConcurrentDictionary<ulong, Trade>();
        public delegate void TradeDelegate(Trade o);
        public event TradeDelegate NewTrade;

        //Ссылка на делегат. Иначе за делегатом придёт заботливый GC
        private Ext.TradeStatusCallback AsyncTradeStatusCallback;



        /// <summary>
        /// Привязывает к Unmanaged событию появления новой сделки в терминале наш делегат (NewTradeEvent)
        /// </summary>
        public void AttachTradeEvent()
        {
            AsyncTradeStatusCallback = new Ext.TradeStatusCallback(NewTradeEvent);
            Ext.SubscribeTrades("", "");//Подписка на все сделки
            Ext.StartTrades(AsyncTradeStatusCallback);
        }

        /// <summary>
        /// Метод вызывается библиотекой Trans2Quik при появлении новой сделки
        /// </summary>
        /// <param name="nMode">0 если новая заявка, не 0 если повтор.</param>
        /// <param name="nNumber">Номер сделки в торговой системе</param>
        /// <param name="nOrderNumber">Номер ордера в торговой системе</param>
        /// <param name="ClassCode">Код класса инструмента</param>
        /// <param name="SecCode">Код инструмента</param>
        /// <param name="dPrice">Цена</param>
        /// <param name="nQty">Количество</param>
        /// <param name="dValue">Сумма</param>
        /// <param name="nIsSell">Если да то сделка - продажа, иначе покупка</param>
        /// <param name="pTradeDescriptor">Дескриптор сделки для получения подробностей</param>
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


            t.Account = Ext.TrdAccount(pTradeDescriptor);
            t.AccruedInt = Ext.TrdAccruedInt(pTradeDescriptor);
            t.AccruedInt2 = Ext.TrdAccuedInt2(pTradeDescriptor);
            t.BlockSec = Ext.TrdBlockSecurities(pTradeDescriptor);
            t.BrokerRef = Ext.TrdBrokerRef(pTradeDescriptor);
            t.ClientCode = Ext.TrdClientCode(pTradeDescriptor);
            t.Ccy = Ext.TrdCcy(pTradeDescriptor);
            t.CommBroker = Ext.TrdBrokerCommission(pTradeDescriptor);
            t.CommClearing = Ext.TrdClearingCommission(pTradeDescriptor);
            t.CommExchange = Ext.TrdExchangeCommisssion(pTradeDescriptor);
            t.CommTradingSys = Ext.TrdTradingSystemCommission(pTradeDescriptor);
            t.CommTS = Ext.TrdTsCommission(pTradeDescriptor);
            t.ExchangeCode = Ext.TrdExchangeCode(pTradeDescriptor);
            t.FirmId = Ext.TrdFirmId(pTradeDescriptor);
            t.IsMarginal = Ext.TrdIsMarginal(pTradeDescriptor) == 1;
            t.Kind = Ext.TrdKind(pTradeDescriptor);
            t.Price2 = Ext.TrdPrice2(pTradeDescriptor);
            t.PartnerFirmId = Ext.TrdPartnerFirmId(pTradeDescriptor);
            t.RepoDisc = Ext.TrdRepoStarDiscount(pTradeDescriptor);
            t.RepoDiscHigh = Ext.TrdRepoUpperDiscount(pTradeDescriptor);
            t.RepoDiscLow = Ext.TrdRepoLowerDiscount(pTradeDescriptor);
            t.RepoRate = Ext.TrdRepoRate(pTradeDescriptor);
            t.RepoTerm = Ext.TrdRepoTerm(pTradeDescriptor);
            t.StationId = Ext.TrdTradeStationId(pTradeDescriptor);
            t.UserId = Ext.TrdUserId(pTradeDescriptor);
            t.Yield = Ext.TrdYield(pTradeDescriptor);
            t.Volume1 = Ext.TrdRepoValue(pTradeDescriptor);
            t.Volume2 = Ext.TrdRepoValue2(pTradeDescriptor);

            //Добавить в кэш
            if (!TradeCache.ContainsKey(t.Number)) TradeCache.TryAdd(t.Number, t);

            //Если не повтор, то вызвать событие
            if (nMode == 0) NewTrade?.Invoke(t);

        }

    }
}
