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
            public Callbacks.Unit?      Height    { get; init; }
            public Callbacks.Unit?      Width     { get; init; }

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

        public Space(Callbacks.Unit width, Spec? spec = null)
            : base(TransformShorthand(spec, width), Classes) { }

        public Space(Callbacks.Unit width, Callbacks.Unit height, Spec? spec = null)
            : base(TransformShorthand(spec, width, height), Classes) { }

        private static BaseSpec TransformShorthand
        (
            Spec? spec, Callbacks.Unit? width = null, Callbacks.Unit? height = null
        ) => new Spec
        {
            IsVisible = spec?.IsVisible,
            Classes   = spec?.Classes,
            Height    = height ?? spec?.Height,
            Width     = width  ?? spec?.Width,
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