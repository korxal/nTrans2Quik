# nTrans2Quik
Библиотека для работы с API импорта транзакций Trans2QuikAPI от ARQA Technologies
## Обзор
Эта библиотека предназначена для взаимодействия с торговым терминалом Quik от ARQA Technologies. Библиотека позволяет отправлять транзакции и получать ордера и сделки.
## Схема взаимодействия:

![Integration-Schema](https://raw.githubusercontent.com/korxal/nTrans2Quik/main/Docs/Schema.png "Схема взаимодействия")

## Требования для работы библиотеки:
1. ОС Windows
2. Терминал Quik подключённый к серверу Quik (Брокеру)
3. В терминале должна быть включена обработка внешних транзакций

## Быстрый старт:

1.	Включите в терминале Qui обработку внешних транзакций
![External-Transactions](https://raw.githubusercontent.com/korxal/nTrans2Quik/main/Docs/ExternalTrans.png "Внешние транзакции")
2.	Создайте новое консольное приложение в Visual Studio или Viusal Stuido Code
3.	Подключите Nuget пакет nTrans2Quik
4.	Вставьте в метод Main код ниже:

````c#
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
    InternalId = 1, //Внутренний порядковый номер
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
````

5.	Скачайте Api импорта транзакций с сайта Arqa https://arqatech.com/ru/support/files/ и положите Trans2Quik.dll в директорию с терминалом Quik
6.	Если используется 64битная библиотека, то в настройках проекта снимите галку “Prefer 32 bits” 
![Bitness](https://raw.githubusercontent.com/korxal/nTrans2Quik/main/Docs/32Bits.png "32 бит")
7.	При запуске приложения оно попытается отправить транзакцию в терминал и сообщит о результате

## Подписка на сделки\ордера
Осуществляется при помощи событий NewOrder, NewTrade.
Пример:
````c#
Q.NewTrade += Q_NewTrade;
\\...
 private static void Q_NewTrade(Trade o)
{
    Console.WriteLine($"Пришла новая сделка №{o.Number}");
}
````

## Nuget
https://www.nuget.org/packages/nTrans2Quik/
