using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Abituria.security
{
    public static class SecureStringHelpers///Pomocnik dla klasy SecureString
    {
        public static string Unsecure(this SecureString secureString)///Deszyfruje SecureString
        {
            if (secureString == null)///Upewnij się, że hasło istnieje
            {
                return string.Empty;
            }
            var unmanagedString = IntPtr.Zero;///Weź wskaźnik zdeszyfrowanego hasła z pamięci
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);///Deszyfruje hasło
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);///Czyści jakiekolwiek alokacje z pamięci
            }
        }
    }
}