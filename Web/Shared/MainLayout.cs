using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Colorant;
using Integrant4.Colorant.Themes.Default;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Constructs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;

namespace Web.Shared
{
    public partial class MainLayout
    {
        private VariantLoader _defaultVariantLoader = null!;
        private VariantLoader _solidsVariantLoader  = null!;

        private Header _header = null!;

        private readonly Stopwatch _stopwatch = new();

        protected override void OnInitialized()
        {
            _stopwatch.Start();
            
            _defaultVariantLoader = new VariantLoader(StorageService, new Theme(),
                Variants.Dark.ToString());

            _defaultVariantLoader.OnComplete      += _ =>
            {
                InvokeAsync(StateHasChanged);
                
                _stopwatch.Stop();
            };
            _defaultVariantLoader.OnVariantChange += _ => InvokeAsync(StateHasChanged);

            //

            _solidsVariantLoader = new VariantLoader(StorageService, new Integrant4.Colorant.Themes.Solids.Theme(),
                Integrant4.Colorant.Themes.Solids.Variants.Normal.ToString());

            //

            _header = new Header(() => new IRenderable[]
            {
                new PageLink(() => new IRenderable[]
                {
                    "I4".AsContent(),
                }, new PageLink.Spec(() => "/") {IsTitle = Always.True}),
                new Filler(),
                new PageLink(() => new IRenderable[]
                {
                    "Elements".AsContent(),
                }, new PageLink.Spec(() => "/elements")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com") {IsHighlighted = Always.True}),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new Dropdown
                (
                    () => new IRenderable[]
                    {
                        new PageLink(() => new IRenderable[]
                        {
                            "Dropdown 1".AsContent(),
                            new BootstrapIcon("chevron-down"),
                        }, new PageLink.Spec(() => "/elements")),
                    },
                    () => new IRenderable[]
                    {
                        new TextBlock(() => new IRenderable[] {"Content".AsContent()}),
                        new HorizontalLine(),
                        new TextBlock(() => new IRenderable[] {"Content 2".AsContent()}),
                    }
                ),
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _defaultVariantLoader.Load();

                // Thread.Sleep(100);
                // throw new Exception();
            }
        }
    }
}