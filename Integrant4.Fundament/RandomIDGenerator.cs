using System;

namespace Integrant4.Fundament
{
    public static class RandomIDGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly Random Random = new();

        // https://stackoverflow.com/a/1344258
        public static string Generate()
        {
            var stringChars = new char[8];

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = Chars[Random.Next(Chars.Length)];
            }

            return new string(stringChars);
        }
    }
}