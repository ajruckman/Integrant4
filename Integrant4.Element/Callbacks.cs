using System;
using System.Collections.Generic;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    public static class Callbacks
    {
        public delegate T Callback<out T>();

        //

        public delegate bool IsVisible();

        public delegate bool IsDisabled();

        public delegate bool IsRequired();

        public delegate bool IsChecked();

        public delegate ClassSet Classes();

        public delegate string HREF();

        //

        public delegate Element.Size Size();

        public delegate string Color();

        public delegate double Pixels();

        public delegate Element.Unit Unit();

        public delegate double Scale();

        public delegate double REM();

        public delegate Element.FontWeight FontWeight();

        public delegate Element.TextAlign TextAlign();

        public delegate Element.Display Display();

        public delegate Element.FlexAlign FlexAlign();

        public delegate Element.FlexJustify FlexJustify();

        //

        public delegate string? DataValue();

        public delegate IDictionary<string, DataValue> Data();

        public delegate Element.Tooltip? Tooltip();
    }

    public class Always
    {
        public static bool True()  => true;
        public static bool False() => false;

        /// <summary>
        /// Invokes a function to get a value once, and returns an anonymous method to return the retrieved value.
        /// Useful only if the initial getter takes significant time to invoke.
        /// </summary>
        /// <param name="initialGetter">The function to call to retrieve the value that this method caches.</param>
        /// <typeparam name="T">The type of value to retrieve and to return.</typeparam>
        /// <returns>An anonymous method to retrieve a value that was retrieved only once.</returns>
        public static Func<T> Value<T>(Func<T> initialGetter)
        {
            T value = initialGetter.Invoke();
            return () => value;
        }

        public static Func<T> Value<T>(T value)
        {
            return () => value;
        }
    }
}