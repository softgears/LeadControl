using System;
using System.Security.Cryptography;
using System.Text;

namespace LeadControl.Domain.Misc
{
    /// <summary>
    /// Утилиты для обработки и генерации паролей
    /// </summary>
    public static class PasswordUtils
    {
        /// <summary>
        /// Символы используемые при генерации пароля
        /// </summary>
        private const string PasswordChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM12345678!@#$%^&";

        /// <summary>
        /// Используемый генератор случайных чисел
        /// </summary>
        private static readonly Random RandomGenerator = new Random(System.Environment.TickCount);

        /// <summary>
        /// Генерирует пароль указанной длины
        /// </summary>
        /// <param name="lenght">Желаемая длина пароля</param>
        /// <returns></returns>
        public static string GeneratePassword(int lenght)
        {
            var resultPassword = new StringBuilder();
            while (resultPassword.Length < lenght)
            {
                resultPassword.Append(PasswordChars[RandomGenerator.Next(PasswordChars.Length - 1)]);
            }
            return resultPassword.ToString();
        }

        /// <summary>
        /// Генерирует MD5 хеш переданной строки
        /// </summary>
        /// <param name="inputString">Входная строка</param>
        /// <returns>Пароль</returns>
        public static string GenerateMD5PasswordHash(string inputString)
        {
            var byteArray = Encoding.Unicode.GetBytes(inputString);
            var hash = new MD5CryptoServiceProvider().ComputeHash(byteArray);
            var hashString = new StringBuilder();
            foreach (var sec in hash)
                hashString.Append(sec.ToString("X2"));
            return hashString.ToString();
        }

        /// <summary>
        /// Рекурсивно генерирует хеш строки, указанное количество раз
        /// </summary>
        /// <param name="input">Оригинальная строка</param>
        /// <param name="times">Сколько раз генерировать</param>
        /// <returns></returns>
        public static string Hashify(string input, int times)
        {
            for (var i = 0; i < times; i++)
            {
                input = GenerateMD5PasswordHash(input);
            }
            return input;
        }
    }
}