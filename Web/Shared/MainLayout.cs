using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Colorant;
using Integrant4.Colorant.Themes.Main;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Bits.BitPresets;
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

            _defaultVariantLoader.OnComplete += _ =>
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
                new HeaderTitleLink("Integrant4".AsDynamicContents()),
                new Filler(),
                new HeaderLink(() => new IRenderable[]
                {
                    "Elements".AsContent(),
                }, () => "/elements"),
                new VerticalLine(),
                new Dropdown(() => new IRenderable[]
                {
                    new HeaderLink("Dropdown".AsDynamicContent(), () => "/"),
                }, () => new IRenderable[]
                {
                    new Button(() => new IRenderable[]
                    {
                        "Home".AsContent(),
                    }, new Button.Spec { HREF = () => "/home", Style = () => Button.Style.Transparent }),
                    new Button(() => new IRenderable[]
                    {
                        "Elements".AsContent(),
                    }, new Button.Spec { HREF = () => "/elements", Style = () => Button.Style.Transparent }),
                    new TextBlock(() => new IRenderable[]
                    {
                        "Text block".AsContent(),
                    }),
                }),
                new VerticalLine(),
                new HeaderLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, () => "https://google.com"),
                new VerticalLine(),
                new HeaderLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, () => "https://google.com", new HeaderLink.Spec { IsHighlighted = Always.True }),
                new VerticalLine(),
                new TextBlock(() => new IRenderable[]
                {
                    "Text block".AsContent(),
                }),
                new VerticalLine(),
                new HeaderLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, () => "https://google.com"),
                new VerticalLine(),
                new Dropdown
                (
                    () => new IRenderable[]
                    {
                        new HeaderLink(() => new IRenderable[]
                        {
                            "Dropdown 1".AsContent(),
                            new BootstrapIcon("chevron-down"),
                        }, () => "/elements"),
                    },
                    () => new IRenderable[]
                    {
                        new TextBlock(() => new IRenderable[] { "Content".AsContent() }),
                        new HorizontalLine(),
                        new TextBlock(() => new IRenderable[] { "Content 2".AsContent() }),
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