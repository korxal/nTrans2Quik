using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nTrans2Quik
{
    public static class Utils
    {
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


    }
}
