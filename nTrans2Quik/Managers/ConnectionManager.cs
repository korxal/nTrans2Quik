using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{
    internal class ConnectionManager
    {

        private readonly string QuikPath;

        public ConnectionManager(string QuikFolder) => QuikPath = QuikFolder;

        public delegate void ConnectionStateChangedDelegate(bool IsConnected);
        public event ConnectionStateChangedDelegate ConnectionStateChanged;

        //Ссылка на делегат. Иначе за делегатом придёт заботливый GC
        private Ext.ConnStatusCallBack AsyncConnStatusCallBack;


        /// <summary>
        /// Делегат статуса подключения к Терминалу. Вызывается библиотекой Trans2Quik когда меняется статус подключения.
        /// </summary>
        /// <param name="nConnectionEvent">Статус подключения</param>
        /// <param name="nExtendedErrorCode">Код ошибки (из ОС)</param>
        /// <param name="lpstrInfoMessage">Сообщение терминала</param>
        private void ConnectionStatusDelegate(Int32 nConnectionEvent, UInt32 nExtendedErrorCode, byte[] lpstrInfoMessage)
        {
            //10 = connected
            ConnectionStateChanged?.Invoke(nConnectionEvent == 10);
        }


        /// <summary>
        /// Устанавливает подключение к терминалу Quik 
        /// </summary>
        public void ConnectToQuikTerminal()
        {
            byte[] ErrorMessage = new byte[50];
            Int32 ErrorCode = 0;
            Int32 rez;

            try
            {
                AsyncConnStatusCallBack = new Ext.ConnStatusCallBack(ConnectionStatusDelegate);
                rez = Ext.SetConnectionStatusCallBack(AsyncConnStatusCallBack, ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            }
            catch (BadImageFormatException)
            {
                //Не cмогли подключить unmanaged библиоткеку, давайте разбираться.
                //if(Environment.Is64BitProcess && IntPtr.Size == 4) throw new Exception("А мы точно в Винде и на Intel?") 

                //Environment.Is64BitProcess для excel может не прокатить
                if (IntPtr.Size == 4)
                    //Иди сними кнопку "Prefer 32 Bits"
                    throw new Exception("Ошибка подключения библиотеки Trans2Quik. 32-битный С# модуль попытался загруть 64-битный Trans2Quik.dll");
                else
                    throw new Exception("Ошибка подключения библиотеки Trans2Quik. 64-битный С# модуль попытался загруть 32-битный Trans2Quik.dll");
            }

            //Пара проверок что путь до терминала валидный.
            if (!Directory.Exists(QuikPath)) throw new DirectoryNotFoundException("Путь не найден:" + QuikPath);
            if (!File.Exists(Path.Combine(QuikPath, "info.exe"))) throw new FileNotFoundException("Info.exe не найден в " + QuikPath);

            //Проверить что терминал Квик готов к подключению
            string QuikPipePathIncoming = "*WorkDir#" + QuikPath.TrimEnd('\\').Replace(":", "_").Replace("\\", "#") + "#QUIK_REQUESTS_INCOMING";
            string QuikPipePathOutgoing = "*WorkDir#" + QuikPath.TrimEnd('\\').Replace(":", "_").Replace("\\", "#") + "#QUIK_REQUESTS_OUTCOMING";

            if (Directory.GetFiles("\\\\.\\pipe\\", QuikPipePathIncoming).Length != 1 ||
                Directory.GetFiles("\\\\.\\pipe\\", QuikPipePathOutgoing).Length != 1)
                throw new Exception("Либо терминал не запущен, либо выключена обработка внешних транзакций. Сервисы -> Экспорт\\импорт данных -> внешние транзакции");


            rez = Ext.СonnectToTerminal(QuikPath, ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);

            //231 - ERROR_PIPE_BUSY - Кто-то уже подключен.
            if (ErrorCode == 231) throw new Exception($"Терминал Quik не отвечает, пожалуйста выключите и снова включите обработку внешних транзакий в терминале (Сервисы -> Экспорт\\импорт данных -> внешние транзакции)");
            //109 - ERROR_BROKEN_PIPE  - Терминал скорее всего уже мёртв к этому времени...
            if (ErrorCode == 109) throw new Exception($"Похоже что терминал Quik завис, пожалуйста перезапустите терминал");

            if (rez != 0)
                throw new Exception($"Ошибка подключения к терминалу Quik, код ошибки {ErrorCode}, описание ошибки:'{Utils.ErrorDecode(ErrorCode)}', Сообщение терминала:{Encoding.ASCII.GetString(ErrorMessage).TrimEnd((char)0)}");



            rez = Ext.IsDllConnected(ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            //10 = Подключено к терминалу
            if (rez != 10) throw new Exception("Подключение не установлено");
        }


        /// <summary>
        /// Разрывает подключение к терминалу Quik
        /// </summary>
        public void DisconnectFromQuikTerminal()
        {
            byte[] ErrorMessage = new Byte[255];
            int ErrorCode = 0;
            var rez = Ext.DisconnectFromTerminal(ref ErrorCode, ErrorMessage, (uint)ErrorMessage.Length);
            if (rez != 0) throw new Exception($"Не удалось отключиться от Терминала Quik, код ошибки {rez}, описание ошибки:'{Utils.ErrorDecode(rez)}'");
        }

        /// <summary>
        /// Проверяет статус подключения Терминала к вышестоящему серверу Quik
        /// </summary>
        /// <returns>Вернет true если подключён</returns>
        public bool IsQuikConnectedToServer()
        {
            byte[] ErrorMessage = new Byte[255];
            int ErrorCode = 0;
            var rez = Ext.IsQuikConnected(ref ErrorCode, ErrorMessage, (uint)ErrorMessage.Length);
            if (ErrorCode == 0 && rez == 8) return true;
            return false;
        }

        /// <summary>
        /// Проверяет подключена ли библиотека Trans2Quik к Терминалу 
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToQuikTerminal()
        {
            byte[] ErrorMessage = new byte[1024];
            Int32 ErrorCode = 0;
            Int32 rez;
            rez = Ext.IsDllConnected(ref ErrorCode, ErrorMessage, (UInt32)ErrorMessage.Length);
            return rez == 10;
        }

    }
}
