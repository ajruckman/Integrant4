using System.Collections.Generic;
using Integrant4.API;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    public static class Callbacks
    {
        public delegate T Callback<out T>();

        public delegate IRenderable              BitContent();
        public delegate IEnumerable<IRenderable> BitContents();

        //

        public delegate string   BitURL();
        public delegate bool     BitIsVisible();
        public delegate bool     BitIsChecked();
        public delegate bool     BitIsDisabled();
        public delegate bool     BitIsRequired();
        public delegate ClassSet BitClasses();
        public delegate string   BitID();

        //

        public delegate Size    BitSize();
        public delegate string  BitColor();
        public delegate double  BitREM();
        public delegate ushort  BitWeight();
        public delegate double  BitPixels();
        public delegate Display BitDisplay();

        //

        public delegate string?                        DataValue();
        public delegate IDictionary<string, DataValue> BitData();
        public delegate string?                        BitTooltip();

        //

        public static BitContents AsContents(this BitContent bitContent) => () => new[] {bitContent.Invoke()};
    }

    internal class BitSpec
    {
        internal BitSpec(Callbacks.BitContents contents)
        {
            Contents = contents;
        }

        internal bool IsStatic { get; init; }

        internal Callbacks.BitContents Contents { get; }

        internal Callbacks.BitURL?        URL        { get; init; }
        internal Callbacks.BitIsVisible?  IsVisible  { get; init; }
        internal Callbacks.BitIsChecked?  IsChecked  { get; init; }
        internal Callbacks.BitIsDisabled? IsDisabled { get; init; }
        internal Callbacks.BitIsRequired? IsRequired { get; init; }
        internal Callbacks.BitClasses?    Classes    { get; init; }
        internal Callbacks.BitID?         ID         { get; init; }

        internal Callbacks.BitSize?    Margin          { get; init; }
        internal Callbacks.BitSize?    Padding         { get; init; }
        internal Callbacks.BitREM?     FontSize        { get; init; }
        internal Callbacks.BitWeight?  FontWeight      { get; init; }
        internal Callbacks.BitColor?   ForegroundColor { get; init; }
        internal Callbacks.BitColor?   BackgroundColor { get; init; }
        internal Callbacks.BitPixels?  Height          { get; init; }
        internal Callbacks.BitPixels?  Width           { get; init; }
        internal Callbacks.BitPixels?  WidthMax        { get; init; }
        internal Callbacks.BitDisplay? Display         { get; init; }
        internal Callbacks.DataValue?  DataValue       { get; init; }
        internal Callbacks.BitData?    Data            { get; init; }
        internal Callbacks.BitTooltip? Tooltip         { get; init; }
    }
}