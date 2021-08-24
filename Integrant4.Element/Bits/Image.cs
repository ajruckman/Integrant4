using System;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Image
    {
        public class Spec : IUnifiedSpec
        {
            public Spec(Callbacks.HREF href)
            {
                HREF = href;
            }

            public Callbacks.HREF HREF { get; }

            public Callbacks.IsVisible?           IsVisible { get; init; }
            public Callbacks.Classes?             Classes   { get; init; }
            public Callbacks.Size?                Margin    { get; init; }
            public Callbacks.Size?                Padding   { get; init; }
            public Callbacks.Unit?                Height    { get; init; }
            public Callbacks.Unit?                HeightMax { get; init; }
            public Callbacks.Unit?                Width     { get; init; }
            public Callbacks.Unit?                WidthMax  { get; init; }
            public Callbacks.Callback<ObjectFit>? ObjectFit { get; init; }
            public Callbacks.Display?             Display   { get; init; }
            public Callbacks.Data?                Data      { get; init; }
            public Callbacks.Tooltip?             Tooltip   { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Image)),
                Scaled      = true,
                IsVisible   = IsVisible,
                HREF        = HREF,
                Classes     = Classes,
                Margin      = Margin,
                Padding     = Padding,
                Height      = Height,
                HeightMax   = HeightMax,
                Width       = Width,
                WidthMax    = WidthMax,
                Display     = Display,
                Data        = Data,
                Tooltip     = Tooltip,
            };
        }
    }

    public partial class Image : BitBase
    {
        private readonly Callbacks.Callback<ObjectFit>? _objectFit;

        public Image(Spec spec) : base(spec)
        {
            _objectFit = spec.ObjectFit;
        }

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "img");
                builder.AddAttribute(++seq, "style", _objectFit?.Invoke() switch
                {
                    null                => null,
                    ObjectFit.None      => null,
                    ObjectFit.Fill      => "fill",
                    ObjectFit.Contain   => "contain",
                    ObjectFit.Cover     => "cover",
                    ObjectFit.ScaleDown => "scale-down",
                    _                   => throw new ArgumentOutOfRangeException()
                });
                builder.AddAttribute(++seq, "src", OuterSpec!.HREF!.Invoke());
                builder.CloseElement();
            }

            return Fragment;
        }
    }

    public partial class Image : BitBase
    {
        public enum ObjectFit
        {
            None,
            Fill,
            Contain,
            Cover,
            ScaleDown,
        }
    }
}