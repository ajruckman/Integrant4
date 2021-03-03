using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public partial class Title : BitBase
    {
        public class TitleSpec
        {
            public TitleSpec(Callbacks.BitHREF href)
            {
                HREF = href;
            }

            public Callbacks.BitHREF HREF { get; }

            public Callbacks.BitIsVisible?  IsVisible       { get; init; }
            public Callbacks.BitIsDisabled? IsDisabled      { get; init; }
            public Callbacks.BitID?         ID              { get; init; }
            public Callbacks.BitClasses?    Classes         { get; init; }
            public Callbacks.BitSize?       Margin          { get; init; }
            public Callbacks.BitSize?       Padding         { get; init; }
            public Callbacks.BitColor?      BackgroundColor { get; init; }
            public Callbacks.BitColor?      ForegroundColor { get; init; }
            public Callbacks.BitREM?        FontSize        { get; init; }
            public Callbacks.BitWeight?     FontWeight      { get; init; }
            public Callbacks.BitDisplay?    Display         { get; init; }
            public Callbacks.BitData?       Data            { get; init; }
            public Callbacks.BitTooltip?    Tooltip         { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                HREF            = HREF,
                ID              = ID,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class Title
    {
        private readonly Callbacks.BitContents _contents;

        public Title(Callbacks.BitContent content, TitleSpec? spec = null)
            : this(content.AsContents(), spec)
        {
        }

        public Title(Callbacks.BitContents contents, TitleSpec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E.Bit", "I4E.Bit." + nameof(Title)))
        {
            _contents = contents;
        }
    }

    public partial class Title
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", Spec.HREF!.Invoke());

                BitBuilder.ApplyAttributes(this, builder, ref seq, null, null);

                foreach (IRenderable renderable in _contents.Invoke())
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}