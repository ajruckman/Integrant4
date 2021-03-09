using System;

namespace Integrant4.Element
{
    public readonly struct Tooltip
    {
        public readonly string     Text;
        public readonly ushort?    Delay;
        public readonly Placement? Placement;

        public Tooltip(string text, ushort? delay = 0, Placement? placement = Element.Placement.Top)
        {
            Text      = text;
            Delay     = delay;
            Placement = placement;
        }
    }

    public enum Placement
    {
        Auto,
        TopStart,
        Top,
        TopEnd,
        RightStart,
        Right,
        RightEnd,
        BottomEnd,
        Bottom,
        BottomStart,
        LeftEnd,
        Left,
        LeftStart,
    }

    public static class PlacementExtensions
    {
        public delegate Placement PlacementGetter();

        public static string Map(this Placement p) => p switch
        {
            Placement.Auto        => "auto",
            Placement.TopStart    => "top-start",
            Placement.Top         => "top",
            Placement.TopEnd      => "top-end",
            Placement.RightStart  => "right-start",
            Placement.Right       => "right",
            Placement.RightEnd    => "right-end",
            Placement.BottomEnd   => "bottom-end",
            Placement.Bottom      => "bottom",
            Placement.BottomStart => "bottom-start",
            Placement.LeftEnd     => "left-end",
            Placement.Left        => "left",
            Placement.LeftStart   => "left-start",
            _                     => throw new ArgumentOutOfRangeException(nameof(p), p, null),
        };
    }
}