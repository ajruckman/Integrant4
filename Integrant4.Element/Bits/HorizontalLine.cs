using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class HorizontalLine : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible? IsVisible { get; init; }
            public Callbacks.Size?      Margin    { get; init; }
            public Callbacks.Color?     Color     { get; init; }
            public Callbacks.Unit?      Height    { get; init; }
            public Callbacks.Unit?      Width     { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
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
        private static readonly ClassSet Classes = new("I4E-Bit", "I4E-Bit-HorizontalLine");

        public HorizontalLine(Spec? spec = null)
            : base(spec?.ToBaseSpec(), Classes) { }

        public HorizontalLine(Callbacks.Size margin, Spec? spec = null)
            : base(TransformShorthand(spec, margin), Classes) { }

        public HorizontalLine(Callbacks.Size margin, Callbacks.Unit width, Spec? spec = null)
            : base(TransformShorthand(spec, margin, width), Classes) { }

        private static BaseSpec TransformShorthand
        (
            Spec? spec, Callbacks.Size? margin = null, Callbacks.Unit? width = null
        ) => new Spec
        {
            IsVisible = spec?.IsVisible,
            Margin    = margin ?? spec?.Margin,
            Color     = spec?.Color,
            Height    = spec?.Height,
            Width     = width ?? spec?.Width,
        }.ToBaseSpec();
    }

    public partial class HorizontalLine
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(this, builder, ref seq, null, null);

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}