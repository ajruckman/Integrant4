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
            public Callbacks.Pixels?    Height    { get; init; }
            public Callbacks.Pixels?    Width     { get; init; }

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
        public VerticalLine(Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-VerticalLine"))
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