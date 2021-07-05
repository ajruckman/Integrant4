using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

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
        private readonly DynamicContents           _contents;
        private readonly Callbacks.HREF            _href;
        private readonly Callbacks.Callback<bool>? _isTitle;
        private readonly Callbacks.Callback<bool>? _isHighlighted;
        private readonly bool                      _doAutoHighlight;

        private bool? _isCurrentPage;

        public HeaderLink(DynamicContent content, Callbacks.HREF href, Spec? spec = null)
            : this(content.AsDynamicContents(), href, spec)
        {
        }

        public HeaderLink(DynamicContents contents, Callbacks.HREF href, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(HeaderLink)))
        {
            _contents = contents;
            _href     = href;
            _isTitle  = spec?.IsTitle;

            if (spec?.IsHighlighted == null)
            {
                _isHighlighted   = () => _isCurrentPage == true;
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

        private async Task CheckPage(IJSRuntime jsRuntime, NavigationManager navMgr, ElementService elementService)
        {
            string currentURL = "/" + navMgr.ToBaseRelativePath(navMgr.Uri);
            string href       = _href.Invoke();

            bool isCurrentPage = currentURL == href;

            if (isCurrentPage != _isCurrentPage)
            {
                _isCurrentPage = isCurrentPage;
                await Interop.HighlightHeaderLink(jsRuntime, elementService.CancellationToken, ID, isCurrentPage);
            }
        }
    }

    public partial class HeaderLink
    {
        private class Component : ComponentBase
        {
            [Parameter] public HeaderLink HeaderLink { get; set; } = null!;

            [Inject] public IJSRuntime        JSRuntime         { get; set; } = null!;
            [Inject] public NavigationManager NavigationManager { get; set; } = null!;
            [Inject] public ElementService    ElementService    { get; set; } = null!;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                IRenderable[] contents = HeaderLink._contents.Invoke().ToArray();

                List<string> ac = new();

                if (HeaderLink._isTitle?.Invoke() == true)
                    ac.Add("I4E-Bit-HeaderLink--Title");

                if (HeaderLink._isHighlighted?.Invoke() == true)
                    ac.Add("I4E-Bit-HeaderLink--Highlighted");

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", HeaderLink._href.Invoke());

                BitBuilder.ApplyAttributes(HeaderLink, builder, ref seq, ac.ToArray(), null);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-HeaderLink-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (!firstRender || !HeaderLink._doAutoHighlight) return;

                NavigationManager.LocationChanged +=
                    async (_, _) => await HeaderLink.CheckPage(JSRuntime, NavigationManager, ElementService);

                await HeaderLink.CheckPage(JSRuntime, NavigationManager, ElementService);
            }
        }
    }
}