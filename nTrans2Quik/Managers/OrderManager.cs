using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{
    public class OrderManager
    {

        /// <summary>
        /// Получить ордер из кэша по номеру ордера
        /// </summary>
        /// <param name="OrderNumber">Номер одрдера</param>
        /// <returns></returns>
        public Order GetOrder(ulong OrderNumber) => OrderCache.ContainsKey(OrderNumber) ? OrderCache[OrderNumber] : null;
        public ConcurrentDictionary<ulong, Order> OrderCache = new ConcurrentDictionary<ulong, Order>();
        public delegate void OrderDelegate(Order o);
        public event OrderDelegate NewOrder;

        //Ссылка на делегат. Иначе за делегатом придёт заботливый GC
        private Ext.OrderStatusCallback AsyncOrderStatusCallback;



        /// <summary>
        /// Привязывает к Unmanaged событию появления новой заявке в терминале наш делегат (NewOrderEvent)
        /// </summary>
        public void AttachOrderEvent()
        {
            AsyncOrderStatusCallback = new Ext.OrderStatusCallback(NewOrderEvent);
            Ext.SubscribeOrders("", "");//Подписка на все ордера
            Ext.StartOrders(AsyncOrderStatusCallback);
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

            o.Qty = Ext.OrdQty(pOrderDescriptor);
            o.Yield = Ext.OrdYield(pOrderDescriptor);
            o.VisibleQty = Ext.OrdVisibleQty(pOrderDescriptor);
            o.ClientCode = Ext.OrdClientCode(pOrderDescriptor);
            o.Account = Ext.OrdAccount(pOrderDescriptor);
            o.AccruedInt = Ext.OrdAccuredInt(pOrderDescriptor);
            o.AwgPrice = Ext.OrdAWGPrice(pOrderDescriptor);
            o.BrokerRef = Ext.OrdBrokerRef(pOrderDescriptor);
            o.ExecType = Ext.OrdExecType(pOrderDescriptor);
            o.ExtendedFlags = Ext.OrdExtendedFlags(pOrderDescriptor);
            o.MinQty = Ext.OrdMinQty(pOrderDescriptor);
            o.RejectReason = Ext.OrdRejectReason(pOrderDescriptor);
            o.UID = Ext.OrdUid(pOrderDescriptor);
            o.UserId = Ext.OrdUserId(pOrderDescriptor);
            o.Period = Ext.OrdPeriod(pOrderDescriptor);
            o.ActivationDate = Utils.FromArqaTime(Ext.OrdActivationTime(pOrderDescriptor));
            o.ExpiryDate = Utils.FromArqaTime(Ext.OrdExpiry(pOrderDescriptor));
            o.WithdrawalDate = Utils.FromArqaTime(Ext.OrdWithdrawTime(pOrderDescriptor));
            o.Date = Utils.FromArqaDealDate(Ext.OrdDateTime(pOrderDescriptor, 0), Ext.OrdDateTime(pOrderDescriptor, 1), Ext.OrdDateTime(pOrderDescriptor, 2));

            //Добавить в кэш
            if (!OrderCache.ContainsKey(o.OrderNum)) OrderCache.TryAdd(o.OrderNum, o);

            //Если не повтор, то вызвать событие
            if (nMode == 0) NewOrder?.Invoke(o);

        }

    }
}
