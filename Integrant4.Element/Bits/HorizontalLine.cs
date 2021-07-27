using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class HorizontalLine : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : UnifiedSpec
        {
            public Callbacks.IsVisible? IsVisible { get; init; }
            public Callbacks.Size?      Margin    { get; init; }
            public Callbacks.Color?     Color     { get; init; }
            public Callbacks.Unit?      Height    { get; init; }
            public Callbacks.Unit?      Width     { get; init; }

            internal override SpecSet ToSpec() => new()
            {
                BaseClasses     = new("I4E-Bit", "I4E-Bit-HorizontalLine"),
                IsVisible       = IsVisible,
                Margin          = Margin,
                BackgroundColor = Color,
                Height          = Height,
                Width           = Width,
            };
        }
    }

    public partial class HorizontalLine
    {
        public HorizontalLine(Spec? spec = null)
            : base(spec) { }

        public HorizontalLine(Callbacks.Size margin, Spec? spec = null)
            : base(TransformShorthand(spec, margin)) { }

        public HorizontalLine(Callbacks.Size margin, Callbacks.Unit width, Spec? spec = null)
            : base(TransformShorthand(spec, margin, width)) { }

        private static Spec TransformShorthand
        (
            Spec? spec, Callbacks.Size? margin = null, Callbacks.Unit? width = null
        ) => new()
        {
            IsVisible = spec?.IsVisible,
            Margin    = margin ?? spec?.Margin,
            Color     = spec?.Color,
            Height    = spec?.Height,
            Width     = width ?? spec?.Width,
        };
    }

    public partial class HorizontalLine
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