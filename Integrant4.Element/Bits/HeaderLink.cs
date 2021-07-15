using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public partial class HeaderLink : BitBase
    {
        public class Spec
        {
            public Callbacks.Callback<bool>? IsTitle       { get; init; }
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
                Classes    = Classes,
                Margin     = Margin,
                Padding    = Padding,
                Data       = Data,
                Tooltip    = Tooltip,
            };
        }
    }

    public partial class HeaderLink
    {
        private readonly DynamicContent            _content;
        private readonly Callbacks.HREF            _href;
        private readonly Callbacks.Callback<bool>? _isTitle;
        private readonly Callbacks.Callback<bool>? _isHighlighted;
        private readonly bool                      _doAutoHighlight;

        public HeaderLink(DynamicContent content, Callbacks.HREF href, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(HeaderLink)))
        {
            _content = content;
            _href    = href;
            _isTitle = spec?.IsTitle;

            if (spec?.IsHighlighted == null)
            {
                _doAutoHighlight = true;
            }
            else
            {
                _isHighlighted   = spec.IsHighlighted;
                _doAutoHighlight = false;
            }
        }
    }

    public partial class HeaderLink
    {
        public override RenderFragment Renderer() => builder =>
        {
            builder.OpenComponent<Component>(0);
            builder.AddAttribute(1, "HeaderLink", this);
            builder.CloseComponent();
        };
    }

    public partial class HeaderLink
    {
        private class Component : ComponentBase
        {
            [Parameter] public HeaderLink HeaderLink { get; set; } = null!;

            [Inject] public ElementService ElementService { get; set; } = null!;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                string href = HeaderLink._href.Invoke();

                List<string> ac = new();

                if (HeaderLink._isTitle?.Invoke() == true)
                    ac.Add("I4E-Bit-HeaderLink--Title");

                if (HeaderLink._isHighlighted?.Invoke() == true)
                    ac.Add("I4E-Bit-HeaderLink--Highlighted");

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", href);

                BitBuilder.ApplyAttributes(HeaderLink, builder, ref seq, ac.ToArray(), null);

                foreach (IRenderable renderable in HeaderLink._content.GetAll())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-HeaderLink-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                //

                if (HeaderLink._doAutoHighlight)
                    ElementService.RegisterLink(HeaderLink.ID, href);
            }
        }
    }
}