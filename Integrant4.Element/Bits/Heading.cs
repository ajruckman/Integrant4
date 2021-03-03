using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Heading : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class HeadingSpec
        {
            public SizeGetter? Size { get; init; }

            public Callbacks.BitIsVisible? IsVisible       { get; init; }
            public Callbacks.BitID?        ID              { get; init; }
            public Callbacks.BitClasses?   Classes         { get; init; }
            public Callbacks.BitSize?      Margin          { get; init; }
            public Callbacks.BitColor?     BackgroundColor { get; init; }
            public Callbacks.BitColor?     ForegroundColor { get; init; }
            public Callbacks.BitDisplay?   Display         { get; init; }
            public Callbacks.BitData?      Data            { get; init; }
            public Callbacks.BitTooltip?   Tooltip         { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                IsVisible       = IsVisible,
                ID              = ID,
                Classes         = Classes,
                Margin          = Margin,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class Heading
    {
        private readonly Callbacks.BitContents _contents;

        public Heading(Callbacks.BitContent content, HeadingSpec? spec = null)
            : this(content.AsContents(), spec)
        {
        }

        public Heading(Callbacks.BitContents contents, HeadingSpec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E.Bit", "I4E.Bit." + nameof(Heading)))
        {
            _contents   = contents;
            _sizeGetter = spec?.Size ?? DefaultSizeGetter;
        }
    }

    public partial class Heading
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "h" + (int) _sizeGetter.Invoke());

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

    public partial class Heading
    {
        public delegate Size SizeGetter();

        public enum Size
        {
            H1 = 1, H2 = 2, H3 = 3, H4 = 4, H5 = 5, H6 = 6,
        }

        private readonly SizeGetter _sizeGetter;

        private static SizeGetter DefaultSizeGetter => () => Size.H1;
    }
}