using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public partial class Link : BitBase
    {
        public class Spec
        {
            public Spec(Callbacks.BitHREF href)
            {
                HREF = href;
            }

            public Callbacks.BitHREF HREF { get; }

            public Callbacks.Callback<bool>? IsAccented { get; init; }
            public Callbacks.Callback<bool>? IsButton   { get; init; }

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
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                HREF            = HREF,
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

    public partial class Link
    {
        private readonly Callbacks.BitContents     _contents;
        private readonly Callbacks.Callback<bool>? _accented;
        private readonly Callbacks.Callback<bool>? _isButton;

        public Link(Callbacks.BitContent content, Spec spec)
            : this(content.AsContents(), spec)
        {
        }

        public Link(Callbacks.BitContents contents, Spec spec)
            : base(spec?.ToBitSpec(), new ClassSet("I4E.Bit", "I4E.Bit." + nameof(Link)))
        {
            _contents = contents;
            _accented = spec?.IsAccented;
            _isButton = spec?.IsButton;
        }
    }

    public partial class Link
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                IRenderable[] contents = _contents.Invoke().ToArray();

                List<string> ac = new();

                if (_accented?.Invoke() == true)
                    ac.Add("I4E.Bit.Link--Accented");

                if (_isButton?.Invoke() == true)
                {
                    ac.Add("I4E.Bit.Link--Button");

                    if (contents.First() is IIcon) ac.Add("I4E.Bit.Link--Button--IconLeft");
                    if (contents.Last() is IIcon) ac.Add("I4E.Bit.Link--Button--IconRight");
                }

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", BaseSpec.HREF!.Invoke());

                BitBuilder.ApplyAttributes(this, builder, ref seq, ac.ToArray(), null);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E.Bit.Button.Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}