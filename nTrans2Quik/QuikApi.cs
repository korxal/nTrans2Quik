using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace nTrans2Quik
{
    public class Trans2Quik : IDisposable
    {

        private readonly ConnectionManager cm;
        private readonly OrderManager om;
        private readonly TradeManager tm;
        private readonly TransactionManager trm;

    

        public Trans2Quik(string QuikTrminalFolder)
        {
            //Путь к Trans2Quik.dll
            string Trans2QuikDllPath = Path.Combine(QuikTrminalFolder, Ext.Lib);

            if (!File.Exists(Trans2QuikDllPath)) throw new Exception("TRANS2QUIK.DLL не найден в " + QuikTrminalFolder);

            //Пытаемся загрузить в память Trans2Quik.dll
            Ext.Trans2QuikPtr = Ext.LoadLibrary(Trans2QuikDllPath);
            if (Ext.Trans2QuikPtr == IntPtr.Zero)
            {
                //Если не вышло, ищем номер ошибки ОС
                var error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                if (error == 193)//Несовпадение битности
                {
                    if (IntPtr.Size == 4)
                        //Иди сними кнопку "Prefer 32 Bits"
                        throw new Exception("Ошибка подключения библиотеки Trans2Quik. 32-битный С# модуль попытался загруть 64-битный Trans2Quik.dll");
                    else
                        throw new Exception("Ошибка подключения библиотеки Trans2Quik. 64-битный С# модуль попытался загруть 32-битный Trans2Quik.dll");
                }

                throw new Exception($"Не смогли загрузить библиотеку Trans2Quik: {error}");
            }

            cm = new ConnectionManager(QuikTrminalFolder);
            tm = new TradeManager();
            trm = new TransactionManager();
            om = new OrderManager();
        }



        public void ConnectToQuikTerminal()
        {
            //Подключиться к терминалу Quik
            cm.ConnectToQuikTerminal();

            //Подцепить обработчик транзакций
            trm.AttachTransactionEvent();

            //Подцепить обработчик ордеров
            om.AttachOrderEvent();

            //Подцепить обработчик сделок
            tm.AttachTradeEvent();

           
        }



        #region Proxies
        public List<Trade> GetOrderTrades(ulong OrderNumber) => om.OrderCache.ContainsKey(OrderNumber) ? tm.TradeCache.Values.Where(x => x.OrderNumber == OrderNumber).ToList() : null;
        public Order GetOrder(ulong number) => om.GetOrder(number);
        public Trade GetTrade(ulong number) => tm.GetTrade(number);
        public Transaction GetTransactionB(uint TranId) => trm.GetTransaction(TranId);


        object key = new object();
        public event OrderManager.OrderDelegate NewOrder { add { lock (key) om.NewOrder += value; } remove { lock (key) om.NewOrder -= value; } }
        public event TransactionManager.TransactionDelegate NewTransaction { add { lock (key) trm.NewTransaction += value; } remove { lock (key) trm.NewTransaction -= value; } }
        public event TradeManager.TradeDelegate NewTrade { add { lock (key) tm.NewTrade += value; } remove { lock (key) tm.NewTrade -= value; } }


        public Transaction SendTransaction(Transaction trn) => trm.SendTransaction(trn);
        public void DisconnectFromQuikTerminal() => cm.DisconnectFromQuikTerminal();



        #endregion

        public void Dispose()
        {

            try { Ext.UnsubscribeOrders(); } catch { }
            try { Ext.UnsubscribeTrades(); } catch { }
            try { cm.DisconnectFromQuikTerminal(); } catch { }

            //Выгрузить Trans2Quik.dll
            if (Ext.Trans2QuikPtr != IntPtr.Zero) Ext.FreeLibrary(Ext.Trans2QuikPtr);

        }
    }
}