using System;

namespace Integrant4.Fundament
{
    public static class Functions
    {
        public static void EnsureOneNotNull<TValue>(string messageIfFalse, params TValue?[] values)
            where TValue : class
        {
            var one = false;

            foreach (TValue? value in values)
            {
                if (value != null)
                {
                    if (one) throw new Exception(messageIfFalse);

                    one = true;
                }
            }

            if (!one) throw new Exception(messageIfFalse);
        }
    }
}