using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestingStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a password");
            string password = Console.ReadLine();
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.HashPassword(password, salt);

            Console.WriteLine("3 hours later....");
            Console.WriteLine("Please enter your password");
            string passwordCheck = Console.ReadLine();
            bool doesWork = PasswordHelper.PasswordMatch(passwordCheck, salt, hash);

            if (doesWork)
            {
                Console.WriteLine("Yay, you are now logged in");
            }
            else
            {
                Console.WriteLine("Invalid password");
            }

            Console.ReadKey(true);
        }
    }

    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[10];
            provider.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            SHA256Managed crypt = new SHA256Managed();
            string combinedString = password + salt;
            byte[] combined = Encoding.Unicode.GetBytes(combinedString);

            byte[] hash = crypt.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }

        public static bool PasswordMatch(string userInput, string salt, string passwordHash)
        {
            string userInputHash = HashPassword(userInput, salt);
            return passwordHash == userInputHash;
        }
    }
}
