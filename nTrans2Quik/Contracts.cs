using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{

    public enum Side
    {
        Buy = 1,
        Sell = 2
    }

    public enum TranType
    {
        New = 1,
        Kill = 2
    }

    public enum ExecCondiotion
    {
        Queue = 1,        //Поставить в очередь
        FillOrKill = 1,   // Полностью или отклонить
        KillBalance = 3   //Снять остаток
    }

    public class Trade
    {

        public int Mode;
        public ulong Number;
        public ulong OrderNumber;
        public string SecCode;
        public string ClassCode;
        public double Price;
        public double Volume1;
        public double Volume2;
        public double Value;

        public Side Side;
        public long Qty;
        public DateTime SettleDate;
        public bool IsMarginal;
        public double AccruedInt;
        public double Yield;
        public double CommClearing;
        public double CommTS;
        public double CommTradingSys;
        public double CommExchange;
        public double CommBroker;
        public double Price2;
        public double AccruedInt2;
        public double RepoRate;
        public double RepoDiscLow;
        public double RepoDiscHigh;
        public double RepoDisc;
        public int RepoTerm;
        public int BlockSec;
        public int Period;
        public int Kind;
        public string Ccy;
        public string SettleCcy;
        public string SettleCode;
        public string Account;
        public string BrokerRef;
        public string ClientCode;
        public string UserId;
        public string FirmId;
        public string PartnerFirmId;
        public string ExchangeCode;
        public string StationId;
        public int TransactionId;


    }

    public class Order
    {

        public bool IsNew;
        public Side Side;
        public ulong OrderNum;
        public int TransactionId;
        public double Qty;
        public DateTime Date;
        public DateTime? ActivationDate;
        public DateTime? WithdrawalDate;
        public DateTime? ExpiryDate;
        public double AccruedInt;
        public double Yield;
        public int UID;
        public double VisibleQty;
        public int Period;
        public int ExtendedFlags;
        public double MinQty;
        public int ExecType;
        public double AwgPrice;
        public string UserId;
        public string Account;
        public string BrokerRef;
        public string ClientCode;
        public string RejectReason;
        public string ClassCode;
        public string SecCode;
        public double Price;
        public double Balance;
        public double Value;
        public int Status;
    }


    public class Transaction
    {

        public ExecCondiotion ExecutionCondition;


        /// <summary>
        /// Внутренний ID транзакции
        /// </summary>
        public uint InternalId;

        /// <summary>
        /// Тип транзакции (Новый ордер, отмена, изменение)
        /// </summary>
        public TranType TransactionType;

        /// <summary>
        /// Номер ордера в торговой системе. Заполняется по результатм транзакции
        /// </summary>
        public ulong OrderNum;

        /// <summary>
        /// Код класса, например TQBR
        /// </summary>
        public string ClassCode;

        /// <summary>
        /// Код ценной бумаги, например HYDR
        /// </summary>
        public string SecCode;

        /// <summary>
        /// Направление сделки, покупка или продажа
        /// </summary>
        public Side Side;

        /// <summary>
        /// Цена заявки
        /// </summary>
        public double Price;

        /// <summary>
        /// Счёт заявки. Брокерский обычно "L01-00000F00"
        /// </summary>
        public string Account;

        /// <summary>
        /// Количество
        /// </summary>
        public double Qty;

        /// <summary>
        /// Код клиента в Quik
        /// </summary>
        public string ClientCode;

        /// <summary>
        /// Коментарий к ордеры
        /// </summary>
        public string Comment;

        /// <summary>
        /// Сообщение терминала, Заполняется по результатм транзакции
        /// </summary>
        public string TerminalMessage;

        /// <summary>
        /// Сообщение об ошибке терминала, Заполняется по результатм транзакции
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// Заполняется по результатм транзакции.
        /// </summary>
        public int Status;

        public double Balance;

    }


    public partial class Trans2Quik
    {
        public static string ErrorDecode(int Result)
        {
            switch (Result)
            {
                case 0: return "Успешно";
                case 1: return "Ошибка";
                case 2: return "Терминал Quik не найден";
                case 3: return "Терминал Quik не поддерживает эту версию Trans2Quik";
                case 4: return "Уже подключен к Quik";
                case 5: return "Ошибка синтаксиса";
                case 6: return "Терминал не подключен к серверу Quik";
                case 7: return "Нет подключения к терминалу Quik";
                case 8: return "Терминал подключён к серверу Quik";
                case 9: return "Терминал отключён от сервера Quik";
                case 10: return "Есть подключение к терминалу Quik";
                case 11: return "Нет подключение к терминалу Quik";
                case 12: return "Ошибка выделения памяти Trans2Quik";
                case 13: return "Неверный хэндл Trans2Quik";
                case 14: return "Ошибка параметров Trans2Quik";
                default: return "Неизвестная ошибка";
            }
        }
    }


}
