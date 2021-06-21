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
            public Callbacks.IsVisible? IsVisible { get; init; }
            public Callbacks.Classes?   Classes   { get; init; }
            public Callbacks.Pixels?    Height    { get; init; }
            public Callbacks.Pixels?    Width     { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
                Classes   = Classes,
                Height    = Height,
                Width     = Width,
            };
        }
    }

    public partial class Space
    {
        private static readonly ClassSet Classes = new("I4E-Bit", "I4E-Bit-" + nameof(Space));

        public Space(Spec? spec = null)
            : base(spec?.ToBaseSpec(), Classes) { }

        public Space(double width)
            : base(TransformWidthShorthand(width), Classes) { }

        public Space(double width, double height)
            : base(TransformWidthHeightShorthand(width, height), Classes) { }

        private static BaseSpec TransformWidthShorthand(double width) => new Spec
        {
            Width = () => width,
        }.ToBaseSpec();

        private static BaseSpec TransformWidthHeightShorthand(double width, double height) => new Spec
        {
            Width  = () => width,
            Height = () => height,
        }.ToBaseSpec();
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