using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class VerticalLine : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IUnifiedSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.IsVisible? IsVisible { get; init; }
            public Callbacks.Size?      Margin    { get; init; }
            public Callbacks.Color?     Color     { get; init; }
            public Callbacks.Unit?      Height    { get; init; }
            public Callbacks.Unit?      Width     { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses     = new("I4E-Bit", "I4E-Bit-VerticalLine"),
                IsVisible       = IsVisible,
                Margin          = Margin,
                BackgroundColor = Color,
                Height          = Height,
                Width           = Width,
            };
        }
    }

    public partial class VerticalLine
    {
        public VerticalLine(Spec? spec = null) : base(spec ?? Spec.Default)
        {
        }

        public VerticalLine(Callbacks.Size margin, Spec? spec = null)
            : base(TransformShorthand(spec, margin))
        {
        }

        public VerticalLine(Callbacks.Size margin, Callbacks.Unit height, Spec? spec = null)
            : base(TransformShorthand(spec, margin, height))
        {
        }

        private static Spec TransformShorthand
        (
            Spec? spec, Callbacks.Size? margin = null, Callbacks.Unit? height = null
        ) => new Spec
        {
            IsVisible = spec?.IsVisible,
            Margin    = margin ?? spec?.Margin,
            Color     = spec?.Color,
            Height    = height ?? spec?.Height,
            Width     = spec?.Width,
        };
    }

    public partial class VerticalLine
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyOuterAttributes(this, builder, ref seq);

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}