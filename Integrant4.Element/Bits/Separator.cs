using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Separator : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible? IsVisible { get; init; }
            public Callbacks.Classes?   Classes   { get; init; }
            public Callbacks.Size?      Margin    { get; init; }
            public Callbacks.Unit?      Height    { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
                Classes   = Classes,
                Margin    = Margin,
                Height    = Height,
            };
        }
    }

    public partial class Separator
    {
        public Separator(Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Separator))) { }
    }

    public partial class Separator
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