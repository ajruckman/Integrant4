using System;

namespace Integrant4.Element
{
    public readonly struct Tooltip
    {
        internal readonly string           Text;
        internal readonly ushort?          Delay;
        internal readonly TooltipFollow    Follow;
        internal readonly TooltipPlacement Placement;

        public Tooltip
        (
            string           text,
            ushort?          delay     = 0,
            TooltipFollow    follow    = TooltipFollow.None,
            TooltipPlacement placement = TooltipPlacement.Top
        )
        {
            Text      = text;
            Follow    = follow;
            Delay     = delay;
            Placement = placement;
        }
    }

    public enum TooltipFollow
    {
        None, Vertical, Horizontal, Initial,
    }

    public enum TooltipPlacement
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
        public delegate TooltipPlacement PlacementGetter();

        internal static string? Map(this TooltipFollow f) => f switch
        {
            TooltipFollow.None       => null,
            TooltipFollow.Vertical   => "vertical",
            TooltipFollow.Horizontal => "horizontal",
            TooltipFollow.Initial    => "initial",
            _                        => throw new ArgumentOutOfRangeException(nameof(f), f, null),
        };

        internal static string Map(this TooltipPlacement p) => p switch
        {
            TooltipPlacement.Auto        => "auto",
            TooltipPlacement.TopStart    => "top-start",
            TooltipPlacement.Top         => "top",
            TooltipPlacement.TopEnd      => "top-end",
            TooltipPlacement.RightStart  => "right-start",
            TooltipPlacement.Right       => "right",
            TooltipPlacement.RightEnd    => "right-end",
            TooltipPlacement.BottomEnd   => "bottom-end",
            TooltipPlacement.Bottom      => "bottom",
            TooltipPlacement.BottomStart => "bottom-start",
            TooltipPlacement.LeftEnd     => "left-end",
            TooltipPlacement.Left        => "left",
            TooltipPlacement.LeftStart   => "left-start",
            _                            => throw new ArgumentOutOfRangeException(nameof(p), p, null),
        };
    }
}