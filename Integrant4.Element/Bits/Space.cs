using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Space : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.BitIsVisible? IsVisible { get; init; }
            // public Callbacks.BitID?        ID        { get; init; }
            public Callbacks.BitClasses?   Classes   { get; init; }
            public Callbacks.BitPixels?    Height    { get; init; }
            public Callbacks.BitPixels?    Width     { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                IsVisible = IsVisible,
                // ID        = ID,
                Classes   = Classes,
                Height    = Height,
                Width     = Width,
            };
        }
    }

    public partial class Space
    {
        public Space(Spec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Space)))
        {
        }
    }

    public partial class Space
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(this, builder, ref seq, null, null);

                builder.OpenElement(++seq, "span");
                builder.CloseElement();

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}