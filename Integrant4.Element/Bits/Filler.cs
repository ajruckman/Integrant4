using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Filler : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible? IsVisible { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
            };
        }
    }

    public partial class Filler
    {
        public Filler(Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-Filler")) { }
    }

    public partial class Filler
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