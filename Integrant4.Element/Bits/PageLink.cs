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
    public partial class PageLink : BitBase
    {
        public class Spec
        {
            public Spec(Callbacks.HREF href)
            {
                HREF = href;
            }

            public Callbacks.HREF HREF { get; }

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
        private readonly DynamicContents           _contents;
        private readonly Callbacks.Callback<bool>? _isTitle;
        private readonly Callbacks.Callback<bool>? _isHighlighted;
        private readonly bool                      _doAutoHighlight;

        private bool? _isCurrentPage;

        public PageLink(DynamicContent content, Spec spec)
            : this(content.AsDynamicContents(), spec) { }

        public PageLink(DynamicContents contents, Spec spec)
            : base(spec.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(PageLink)))
        {
            _contents = contents;
            _isTitle  = spec.IsTitle;

            if (spec.IsHighlighted == null)
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

    public partial class PageLink
    {
        public override RenderFragment Renderer() => builder =>
        {
            builder.OpenComponent<Component>(0);
            builder.AddAttribute(1, "PageLink", this);
            builder.CloseComponent();
        };

        private async Task CheckPage(IJSRuntime jsRuntime, NavigationManager navMgr, ElementService elementService)
        {
            string currentURL = "/" + navMgr.ToBaseRelativePath(navMgr.Uri);
            string href       = BaseSpec.HREF!.Invoke();

            bool isCurrentPage = currentURL == href;

            if (isCurrentPage != _isCurrentPage)
            {
                _isCurrentPage = isCurrentPage;
                await Interop.HighlightPageLink(jsRuntime, elementService.CancellationToken, ID, isCurrentPage);
            }
        }
    }

    public partial class PageLink
    {
        private class Component : ComponentBase
        {
            [Parameter] public PageLink PageLink { get; set; } = null!;

            [Inject] public IJSRuntime        JSRuntime         { get; set; } = null!;
            [Inject] public NavigationManager NavigationManager { get; set; } = null!;
            [Inject] public ElementService    ElementService    { get; set; } = null!;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                IRenderable[] contents = PageLink._contents.Invoke().ToArray();

                List<string> ac = new();

                if (PageLink._isTitle?.Invoke() == true)
                    ac.Add("I4E-Bit-PageLink--Title");

                if (PageLink._isHighlighted?.Invoke() == true)
                    ac.Add("I4E-Bit-PageLink--Highlighted");

                //

                int seq = -1;
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", PageLink.BaseSpec.HREF!.Invoke());

                BitBuilder.ApplyAttributes(PageLink, builder, ref seq, ac.ToArray(), null);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-PageLink-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                // if (PageLink._doAutoHighlight)
                // {
                //     ServiceInjector<IJSRuntime>.Inject(builder, ref seq, v => _jsRuntime = v);
                //     ServiceInjector<NavigationManager>.Inject(builder, ref seq, v =>
                //     {
                //         _navMgr                 =  v;
                //         _navMgr.LocationChanged += async (_, _) => await CheckPage();
                //     });
                // }
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (!firstRender || !PageLink._doAutoHighlight) return;

                NavigationManager.LocationChanged +=
                    async (_, _) => await PageLink.CheckPage(JSRuntime, NavigationManager, ElementService);

                await PageLink.CheckPage(JSRuntime, NavigationManager, ElementService);
            }
        }
    }
}