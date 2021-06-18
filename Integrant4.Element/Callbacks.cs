using System;
using System.Collections.Generic;
using Integrant4.API;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    public static class Callbacks
    {
        public delegate T Callback<out T>();

        public delegate IRenderable BitContent();

        public delegate IEnumerable<IRenderable> BitContents();

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

        //

        public static BitContents AsContents(this BitContent bitContent) => () => new[] { bitContent.Invoke() };
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
    }

    internal class BaseSpec
    {
        internal bool Scaled { get; init; } = false;

        internal Callbacks.IsVisible?  IsVisible  { get; init; }
        internal Callbacks.IsDisabled? IsDisabled { get; init; }
        internal Callbacks.IsRequired? IsRequired { get; init; }
        internal Callbacks.IsChecked?  IsChecked  { get; init; }
        internal Callbacks.Classes?    Classes    { get; init; }
        internal Callbacks.HREF?       HREF       { get; init; }

        internal Callbacks.Size?        Margin          { get; init; }
        internal Callbacks.Size?        Padding         { get; init; }
        internal Callbacks.Color?       BackgroundColor { get; init; }
        internal Callbacks.Color?       ForegroundColor { get; init; }
        internal Callbacks.Color?       HighlightColor  { get; init; }
        internal Callbacks.Pixels?      Height          { get; init; }
        internal Callbacks.Pixels?      HeightMax       { get; init; }
        internal Callbacks.Pixels?      Width           { get; init; }
        internal Callbacks.Pixels?      WidthMax        { get; init; }
        internal Callbacks.Scale?       Scale           { get; init; }
        internal Callbacks.REM?         FontSize        { get; init; }
        internal Callbacks.FontWeight?  FontWeight      { get; init; }
        internal Callbacks.TextAlign?   TextAlign       { get; init; }
        internal Callbacks.Display?     Display         { get; init; }
        internal Callbacks.FlexAlign?   FlexAlign       { get; init; }
        internal Callbacks.FlexJustify? FlexJustify     { get; init; }

        internal Callbacks.Data?    Data    { get; init; }
        internal Callbacks.Tooltip? Tooltip { get; init; }
    }
}