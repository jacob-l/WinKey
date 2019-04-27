using System;
using Microsoft.Win32;

namespace WinKey
{
    public class Program
    {
        private const string KeyPath = @"Software\Microsoft\Windows NT\CurrentVersion";

        private const string DigitalProductId = "DigitalProductId";

        // Possible alpha-numeric characters in product key.
        private const string Digits = "BCDFGHJKMPQRTVWXY2346789";

        public static string DecodeProductKeyWin8AndUp(byte[] digitalProductId)
        {
            var key = "";
            const int keyOffset = 52;
            var isWin8 = (byte)((digitalProductId[66] / 6) & 1);
            digitalProductId[66] = (byte)((digitalProductId[66] & 0xf7) | (isWin8 & 2) * 4);

            var last = 0;
            for (var i = 24; i >= 0; i--)
            {
                var current = 0;
                for (var j = 14; j >= 0; j--)
                {
                    current = current * 256;
                    current = digitalProductId[j + keyOffset] + current;
                    digitalProductId[j + keyOffset] = (byte)(current / 24);
                    current = current % 24;
                    last = current;
                }
                key = Digits[current] + key;
            }
            var keypart1 = key.Substring(1, last);
            const string insert = "N";
            key = key.Substring(1).Replace(keypart1, keypart1 + insert);
            if (last == 0)
            {
                key = insert + key;
            }

            for (var i = 5; i < key.Length; i += 6)
            {
                key = key.Insert(i, "-");
            }
            return key;
        }

        public static string GetWindowsProductKey()
        {
            var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                RegistryView.Default).OpenSubKey(KeyPath);

            if (key == null)
            {
                return "Cannot find key in the registry";
            }

            var digitalProductId = (byte[])key.GetValue(DigitalProductId);

            return DecodeProductKeyWin8AndUp(digitalProductId);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Your key:");
            Console.WriteLine(GetWindowsProductKey());
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
