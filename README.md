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
5.	Скачайте Api импорта транзакций с сайта Arqa https://arqatech.com/ru/support/files/ и положите Trans2Quik.dll в директорию с терминалом Quik
6.	Если используется 64битная библиотека, то в настройках проекта снимите галку “Prefer 32 bits” 
![Bitness](https://raw.githubusercontent.com/korxal/nTrans2Quik/main/Docs/32Bits.png "32 бит")
7.	При запуске приложения оно попытается отправить транзакцию в терминал и сообщит о результате
