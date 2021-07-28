using System;

namespace Integrant4.Element
{
    public readonly struct Unit
    {
        private readonly double _value;
        private readonly Type   _type;

        private Unit(double value, Type type)
        {
            _value = value;
            _type  = type;
        }

        private enum Type
        {
            Pixels, Percentage,
        }

        public static Unit Pixels(double     v) => new(v, Type.Pixels);
        public static Unit Percentage(double v) => new(v, Type.Percentage);

        public string Serialize() => _value + _type switch
        {
            Type.Pixels     => "px",
            Type.Percentage => "%",
            _               => throw new ArgumentOutOfRangeException(),
        };

        public static implicit operator Unit(double v) => Pixels(v);
    }

    public readonly struct Size
    {
        public readonly Unit Top;
        public readonly Unit Right;
        public readonly Unit Bottom;
        public readonly Unit Left;

        public string Serialize() =>
            $"{Top.Serialize()} {Right.Serialize()} {Bottom.Serialize()} {Left.Serialize()}";

        public Size(ushort all)
        {
            Top = Right = Bottom = Left = Unit.Pixels(all);
        }

        public Size(ushort vertical, ushort horizontal)
        {
            Top   = Bottom = Unit.Pixels(vertical);
            Right = Left   = Unit.Pixels(horizontal);
        }

        public Size(ushort top, ushort horizontal, ushort bottom)
        {
            Top    = Unit.Pixels(top);
            Right  = Left = Unit.Pixels(horizontal);
            Bottom = Unit.Pixels(bottom);
        }

        public Size(ushort top, ushort right, ushort bottom, ushort left)
        {
            Top    = Unit.Pixels(top);
            Right  = Unit.Pixels(right);
            Bottom = Unit.Pixels(bottom);
            Left   = Unit.Pixels(left);
        }

        public Size(Unit all)
        {
            Top = Right = Bottom = Left = all;
        }

        public Size(Unit vertical, Unit horizontal)
        {
            Top   = Bottom = vertical;
            Right = Left   = horizontal;
        }

        public Size(Unit top, Unit horizontal, Unit bottom)
        {
            Top    = top;
            Right  = Left = horizontal;
            Bottom = bottom;
        }

        public Size(Unit top, Unit right, Unit bottom, Unit left)
        {
            Top    = top;
            Right  = right;
            Bottom = bottom;
            Left   = left;
        }
    }
}