using System;

namespace Integrant4.Element
{
    public enum Display
    {
        Undefined, Inline, InlineBlock, Block,
    }

    public enum FontWeight
    {
        Thin       = 100,
        ExtraLight = 200,
        Light      = 300,
        Normal     = 400,
        Medium     = 500,
        SemiBold   = 600,
        Bold       = 700,
        ExtraBold  = 800,
        Black      = 900,
    }

    public enum TextAlign
    {
        Left, Center, Right,
    }

    public enum FlexAlign
    {
        Start, Center, End, Stretch, Baseline,
    }

    public enum FlexJustify
    {
        Start, Center, End, SpaceBetween, SpaceAround, SpaceEvenly,
    }

    public static class EnumExtensions
    {
        public static string Serialize(this FlexAlign f) => f switch
        {
            FlexAlign.Start    => "flex-start",
            FlexAlign.Center   => "center",
            FlexAlign.End      => "flex-end",
            FlexAlign.Stretch  => "stretch",
            FlexAlign.Baseline => "baseline",
            _                  => throw new ArgumentOutOfRangeException(nameof(f), f, null),
        };

        public static string Serialize(this FlexJustify f) => f switch
        {
            FlexJustify.Start        => "flex-start",
            FlexJustify.Center       => "center",
            FlexJustify.End          => "flex-end",
            FlexJustify.SpaceBetween => "space-between",
            FlexJustify.SpaceAround  => "space-around",
            FlexJustify.SpaceEvenly  => "space-evenly",
            _                        => throw new ArgumentOutOfRangeException(nameof(f), f, null),
        };
    }
}