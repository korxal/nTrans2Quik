using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections;

namespace nTrans2Quik
{

    public static class Utils
    {


        private readonly static Encoding RU = Encoding.GetEncoding(1251);
        public static string Str(byte[] arr) => RU.GetString(arr).TrimEnd('\0');// Для чтения C строк

        private static volatile int i = 1;
        public static int GetNextTrnId()
        {
            if (i > 2147483640) i = 1; //Лимит Trans2Quik.dll - 2147483646
            return i++;
        }

        public static DateTime? FromArqaDate(int Date)
        {
            if (Date < 1) return null;
            return DateTime.ParseExact(Date.ToString(), "yyyyMMdd", null);
        }

        public static DateTime? FromArqaTime(int Time)
        {
            if (Time < 1) return null;
            return DateTime.ParseExact(Time.ToString(), "HHmmss", null);
        }

        public static DateTime FromArqaDealDate(int Date, int Time, int MCS)
        {
            return FromArqaDate(Date).Value.Add(TimeSpan.ParseExact(Time.ToString(), "hhmmss", null)).Add(TimeSpan.ParseExact(MCS.ToString("000000"), "ffffff", null));
        }

        public static void V(this StringBuilder sb, string Field, object Value)
        {
            switch (Value)
            {
                case null: return;
                case int i when i == 0: return;
                case long i when i == 0: return;
                case uint i when i == 0: return;
                case ulong i when i == 0: return;
                case double i when i == 0: return;
                case decimal i when i == 0: return;
            }
            sb.Append(Field);
            sb.Append("=");
            sb.Append(Value.ToString());
            sb.Append("; ");
        }


        /// <summary>
        /// Маршалер для получения строк из Trans2Quik
        /// </summary>
        public class QuikStrMarshaler : ICustomMarshaler
        {
            static readonly QuikStrMarshaler instance = new QuikStrMarshaler();
            public static ICustomMarshaler GetInstance(string cookie)
            {
                return instance;
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return Marshal.PtrToStringAnsi(pNativeData);
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                return IntPtr.Zero;
            }

            public int GetNativeDataSize()
            {
                return IntPtr.Size;
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                return;
            }

            public void CleanUpManagedData(object ManagedObj)
            {
                return;
            }
        }

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
