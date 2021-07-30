using System;
using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public partial class HeaderLink : BitBase
    {
        public class Spec : IUnifiedSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<bool>?      IsHighlighted { get; init; }
            public Action<HeaderLink, ClickArgs>? OnClick       { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.Classes?    Classes    { get; init; }
            public Callbacks.Size?       Margin     { get; init; }
            public Callbacks.Size?       Padding    { get; init; }
            public Callbacks.Data?       Data       { get; init; }
            public Callbacks.Tooltip?    Tooltip    { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Construct", "I4E-Construct-" + nameof(HeaderLink)),
                IsVisible   = IsVisible,
                IsDisabled  = IsDisabled,
                Classes     = Classes,
                Margin      = Margin,
                Padding     = Padding,
                Data        = Data,
                Tooltip     = Tooltip,
            };
        }
    }

    public partial class HeaderLink
    {
        private readonly ContentRef                _content;
        private readonly Callbacks.HREF            _href;
        private readonly Callbacks.Callback<bool>? _isHighlighted;
        private readonly bool                      _doAutoHighlight;

        public HeaderLink(ContentRef content, Callbacks.HREF href, Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _content = content;
            _href    = href;

            if (spec?.IsHighlighted == null)
            {
                _doAutoHighlight = true;
            }
            else
            {
                _isHighlighted   = spec.IsHighlighted;
                _doAutoHighlight = false;
            }

            if (spec?.OnClick != null)
            {
                OnClick += spec.OnClick;
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

        public event Action<HeaderLink, ClickArgs>? OnClick;

        private void Click(ClickArgs args)
        {
            if (OuterSpec?.IsDisabled?.Invoke() == true) return;

            OnClick?.Invoke(this, args);
        }
    }

    public partial class HeaderLink
    {
        private class Component : ComponentBase
        {
            [Parameter] public HeaderLink        HeaderLink { get; set; } = null!;
            [Parameter] public Action<ClickArgs> OnClick    { get; set; } = null!;

            [Inject] public ElementService ElementService { get; set; } = null!;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                string href = HeaderLink._href.Invoke();

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", href);

                BitBuilder.ApplyOuterAttributes(HeaderLink, builder, ref seq,
                    HeaderLink._isHighlighted?.Invoke() == true
                        ? new[] {"I4E-Construct-HeaderLink--Highlighted"}
                        : null);

                builder.AddAttribute(2, "Click",
                    EventCallback.Factory.Create<MouseEventArgs>(this, Click));

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-HeaderLink-Contents");
                foreach (IRenderable renderable in HeaderLink._content.GetAll())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-HeaderLink-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();

                //

                if (HeaderLink._doAutoHighlight)
                    ElementService.RegisterLink(HeaderLink.ID, href);
            }

            private void Click(MouseEventArgs args)
            {
                var c = new ClickArgs
                (
                    (ushort) args.Button,
                    (ushort) args.ClientX,
                    (ushort) args.ClientY,
                    args.ShiftKey,
                    args.CtrlKey
                );

                OnClick?.Invoke(c);
            }
        }
    }
}