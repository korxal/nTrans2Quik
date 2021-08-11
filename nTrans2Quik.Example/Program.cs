using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik.Example
{


    class Example
    {

        public void GotOrder(Order o)
        {
            string rez = Newtonsoft.Json.JsonConvert.SerializeObject(o);
            Console.WriteLine("Got new order:\r\n" + rez + "\r\n-----------------\r\n\r\n");
        }

        public void GotTransaction(Transaction tr)
        {
            string rez = Newtonsoft.Json.JsonConvert.SerializeObject(tr);
            Console.WriteLine("Got new transaction:\r\n" + rez + "\r\n-----------------\r\n\r\n");
        }

        public void GotTrade (Trade t)
        {
            string rez = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            Console.WriteLine("Got new trade:\r\n" + rez + "\r\n-----------------\r\n\r\n");
        }


        public void Run()
        {
            using (Trans2Quik tc = new Trans2Quik())
            {
                Console.WriteLine("Connected!");

                tc.NewOrder += GotOrder;
                tc.NewTrade += GotTrade;
                tc.NewTransaction += GotTransaction;

                tc.ConnectToQuikTerminal("c:\\Quik");
                System.Threading.Thread.Sleep(5000);

          
                Transaction tr = new Transaction()
                {
                    InternalId = 1,
                    TransactionType = TranType.New,
                    ClassCode = "TQBR",
                    SecCode = "HYDR",
                    Side = Side.Buy,
                    Price = 0.836,
                    Account = "L01-00000F00",
                    Qty = 1,
                    ClientCode = "SUPERCLIENT"
                };

                
                
                var z = tc.SendTransaction(tr);



                Transaction trc = new Transaction()
                {
                    OrderNum = tr.OrderNum+1,
                    TransactionType = TranType.Kill
                };


                var zz = tc.SendTransaction(trc);


                tc.DisconnectFromQuikTerminal();
                Console.WriteLine("Disconnected, press enter to quit.");
            }

            Console.ReadLine();
        }



    }

    class Program
    {
        static void Main(string[] args)
        {
            Example ex = new Example();
            try
            {
                ex.Run();
            }
            catch(Exception e)
            {
                Console.WriteLine("Something went wrong: " + e.Message + "\r\n" + e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
