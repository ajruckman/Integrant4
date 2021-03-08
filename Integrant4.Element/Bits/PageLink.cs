using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public partial class PageLink : BitBase
    {
        public class Spec
        {
            public Spec(Callbacks.HREF href)
            {
                HREF = href;
            }

            public Callbacks.HREF HREF { get; }

            public Callbacks.Callback<bool>? IsHighlighted { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.Classes?    Classes    { get; init; }
            public Callbacks.Size?       Margin     { get; init; }
            public Callbacks.Size?       Padding    { get; init; }
            public Callbacks.Data?       Data       { get; init; }
            public Callbacks.Tooltip?    Tooltip    { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible  = IsVisible,
                IsDisabled = IsDisabled,
                HREF       = HREF,
                Classes    = Classes,
                Margin     = Margin,
                Padding    = Padding,
                Data       = Data,
                Tooltip    = Tooltip,
            };
        }
    }

    public partial class PageLink
    {
        private readonly Callbacks.BitContents     _contents;
        private readonly Callbacks.Callback<bool>? _isHighlighted;

        public PageLink(Callbacks.BitContent content, Spec spec)
            : this(content.AsContents(), spec)
        {
        }

        public PageLink(Callbacks.BitContents contents, Spec spec)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(PageLink)))
        {
            _contents      = contents;
            _isHighlighted = spec?.IsHighlighted;
        }
    }

    public partial class PageLink
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                IRenderable[] contents = _contents.Invoke().ToArray();

                List<string> ac = new();

                if (_isHighlighted?.Invoke() == true)
                    ac.Add("I4E-Bit-PageLink--Highlighted");

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", BaseSpec.HREF!.Invoke());

                BitBuilder.ApplyAttributes(this, builder, ref seq, ac.ToArray(), null);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-PageLink-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}