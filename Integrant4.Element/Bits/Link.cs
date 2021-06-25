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
            public Spec(Callbacks.HREF href)
            {
                HREF = href;
            }

            public Callbacks.HREF HREF { get; }

            public Callbacks.Callback<bool>? IsAccented    { get; init; }
            public Callbacks.Callback<bool>? IsHighlighted { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Pixels?     Height          { get; init; }
            public Callbacks.Pixels?     HeightMax       { get; init; }
            public Callbacks.Pixels?     Width           { get; init; }
            public Callbacks.Pixels?     WidthMax        { get; init; }
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Scaled          = true,
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
                Scale           = Scale,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class Link
    {
        private readonly DynamicContents           _contents;
        private readonly Callbacks.Callback<bool>? _isAccented;
        private readonly Callbacks.Callback<bool>? _isHighlighted;

        public Link(DynamicContent content, Spec spec)
            : this(content.AsDynamicContents(), spec) { }

        public Link(DynamicContents contents, Spec spec)
            : base(spec.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Link)))
        {
            _contents      = contents;
            _isAccented    = spec.IsAccented;
            _isHighlighted = spec.IsHighlighted;
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

                if (_isAccented?.Invoke() == true)
                    ac.Add("I4E-Bit-Link--Accented");

                if (_isHighlighted?.Invoke() == true)
                    ac.Add("I4E-Bit-Link--Highlighted");

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", BaseSpec.HREF!.Invoke());

                BitBuilder.ApplyAttributes(this, builder, ref seq, ac.ToArray(), null);

                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Link-Contents");

                BitBuilder.ApplyContentAttributes(this, builder, ref seq);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Link-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}