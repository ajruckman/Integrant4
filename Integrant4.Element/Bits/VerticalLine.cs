using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class VerticalLine : BitBase
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

    public partial class VerticalLine
    {
        private static readonly ClassSet Classes = new("I4E-Bit", "I4E-Bit-VerticalLine");

        public VerticalLine(Spec? spec = null)
            : base(spec?.ToBaseSpec(), Classes) { }

        public VerticalLine(Callbacks.Size margin, Spec? spec)
            : base(TransformShorthand(spec, margin), Classes) { }

        public VerticalLine(Callbacks.Size margin, Callbacks.Unit height, Spec? spec)
            : base(TransformShorthand(spec, margin, height), Classes) { }

        private static BaseSpec TransformShorthand
        (
            Spec? spec, Callbacks.Size? margin = null, Callbacks.Unit? height = null
        ) => new Spec
        {
            IsVisible = spec?.IsVisible,
            Margin    = margin ?? spec?.Margin,
            Color     = spec?.Color,
            Height    = height ?? spec?.Height,
            Width     = spec?.Width,
        }.ToBaseSpec();
    }

    public partial class VerticalLine
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