using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class TextBlock : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public ElementService? ElementService { get; init; }

            public Callbacks.Callback<bool>? IsHoverable { get; init; }

            public Callbacks.BitIsVisible?  IsVisible       { get; init; }
            public Callbacks.BitIsDisabled? IsDisabled      { get; init; }
            public Callbacks.BitClasses?    Classes         { get; init; }
            public Callbacks.BitSize?       Margin          { get; init; }
            public Callbacks.BitSize?       Padding         { get; init; }
            public Callbacks.BitColor?      BackgroundColor { get; init; }
            public Callbacks.BitColor?      ForegroundColor { get; init; }
            public Callbacks.BitPixels?     Height          { get; init; }
            public Callbacks.BitPixels?     HeightMax       { get; init; }
            public Callbacks.BitPixels?     Width           { get; init; }
            public Callbacks.BitPixels?     WidthMax        { get; init; }
            public Callbacks.BitREM?        FontSize        { get; init; }
            public Callbacks.BitWeight?     FontWeight      { get; init; }
            public Callbacks.BitDisplay?    Display         { get; init; }
            public Callbacks.BitData?       Data            { get; init; }
            public Callbacks.BitTooltip?    Tooltip         { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                ElementService  = ElementService,
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                HeightMax       = HeightMax,
                Width           = Width,
                WidthMax        = WidthMax,
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class TextBlock
    {
        private readonly Callbacks.BitContents     _contents;
        private readonly Callbacks.Callback<bool>? _isHoverable;

        public TextBlock(Callbacks.BitContents contents, Spec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E.Bit", "I4E.Bit." + nameof(TextBlock)))
        {
            _contents    = contents;
            _isHoverable = spec?.IsHoverable;
        }
    }

    public partial class TextBlock
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");

                string[]? ac = _isHoverable?.Invoke() == true ? new[] {"I4E.Bit.TextBlock--Hoverable"} : null;

                BitBuilder.ApplyAttributes(this, builder, ref seq, ac, null);

                foreach (IRenderable renderable in _contents.Invoke())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E.Bit.TextBlock.Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }
                
                builder.CloseElement();
                
                QueueTooltip();
            }

            return Fragment;
        }
    }
}