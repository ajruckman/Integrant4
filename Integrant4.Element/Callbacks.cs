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

        public delegate bool     BitIsVisible();
        public delegate bool     BitIsDisabled();
        public delegate bool     BitIsRequired();
        public delegate bool     BitIsChecked();
        public delegate string   BitID();
        public delegate ClassSet BitClasses();
        public delegate string   BitHREF();

        //

        public delegate Size       BitSize();
        public delegate string     BitColor();
        public delegate double     BitPixels();
        public delegate double     BitREM();
        public delegate FontWeight BitWeight();
        public delegate Display    BitDisplay();

        //

        public delegate string?                        DataValue();
        public delegate IDictionary<string, DataValue> BitData();
        public delegate string?                        BitTooltip();

        //

        public static BitContents AsContents(this BitContent bitContent) => () => new[] {bitContent.Invoke()};
        // public static IEnumerable<IRenderable> ToContents(this IRenderable renderable) => new[] {renderable};
        // public static IEnumerable<IRenderable> ToContents(this Content     content)    => new IRenderable[] {content};
    }

    internal class BitSpec
    {
        // internal BitSpec(Callbacks.BitContents contents)
        // {
        // Contents = contents;
        // }

        internal bool IsStatic { get; init; }

        // internal Callbacks.BitContents Contents { get; }

        internal Callbacks.BitIsVisible?  IsVisible  { get; init; }
        internal Callbacks.BitIsDisabled? IsDisabled { get; init; }
        internal Callbacks.BitIsRequired? IsRequired { get; init; }
        internal Callbacks.BitIsChecked?  IsChecked  { get; init; }
        internal Callbacks.BitID?         ID         { get; init; }
        internal Callbacks.BitClasses?    Classes    { get; init; }
        internal Callbacks.BitHREF?       HREF       { get; init; }

        internal Callbacks.BitSize?    Margin          { get; init; }
        internal Callbacks.BitSize?    Padding         { get; init; }
        internal Callbacks.BitColor?   BackgroundColor { get; init; }
        internal Callbacks.BitColor?   ForegroundColor { get; init; }
        internal Callbacks.BitPixels?  Height          { get; init; }
        internal Callbacks.BitPixels?  HeightMax       { get; init; }
        internal Callbacks.BitPixels?  Width           { get; init; }
        internal Callbacks.BitPixels?  WidthMax        { get; init; }
        internal Callbacks.BitREM?     FontSize        { get; init; }
        internal Callbacks.BitWeight?  FontWeight      { get; init; }
        internal Callbacks.BitDisplay? Display         { get; init; }

        internal Callbacks.BitData?    Data    { get; init; }
        internal Callbacks.BitTooltip? Tooltip { get; init; }
    }
}