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
            public Callbacks.BitIsVisible? IsVisible { get; init; }
            public Callbacks.BitSize?      Margin    { get; init; }
            public Callbacks.BitColor?     Color     { get; init; }
            public Callbacks.BitPixels?    Height    { get; init; }
            public Callbacks.BitPixels?    Width     { get; init; }

            internal BitSpec ToBitSpec() => new()
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
        public VerticalLine(Spec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E-Bit", "I4E-Bit-VerticalLine"))
        {
        }
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