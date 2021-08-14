using System;

namespace nTrans2Quik.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            nTrans2Quik.Trans2Quik Q;
            try
            {
                Q = new nTrans2Quik.Trans2Quik("C:\\Quik");//Путь к терминалу Quik
            }
            catch (Exception e)
            {
                Console.WriteLine("Не смогли найти терминал: " + e.Message);
                Console.ReadLine();
                return;
            }

            Q.ConnectToQuikTerminal(); //Подключаемся к терминалу

            nTrans2Quik.Transaction t = new nTrans2Quik.Transaction() //Готовим транзакцию
            {
                Side = nTrans2Quik.Side.Buy,//Покупка
                ClassCode = "TQBR",//Код класса
                SecCode = "AFLT",//Код инструмента - Аэрофлот
                ClientCode = "657548", //Код клиента у брокера
                Qty = 1, // 1 ЛОТ
                Account = "L01-00000F00" // Счёт. Если брокер то обычно L01-00000F00
            };
            nTrans2Quik.Transaction rez = Q.SendTransaction(t); //Отправляем транзакцию

            Console.WriteLine("Результат:" + rez.TerminalMessage + " " + rez.ErrorMessage);
            Console.ReadLine();
        }
    }
}
