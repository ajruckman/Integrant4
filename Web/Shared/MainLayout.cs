using System.Threading.Tasks;
using Integrant4.Colorant.Services;
using Integrant4.Colorant.Themes.Default;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Web.Shared
{
    public partial class MainLayout
    {
        private VariantLoader _defaultVariantLoader = null!;
        private VariantLoader _solidsVariantLoader  = null!;

        [Inject] public ResourceService ResourceService { get; set; } = null!;

        protected override void OnInitialized()
        {
            _defaultVariantLoader = new VariantLoader(StorageService, new Theme(),
                Variants.Dark.ToString());

            _defaultVariantLoader.OnComplete      += _ => InvokeAsync(StateHasChanged);
            _defaultVariantLoader.OnVariantChange += _ => InvokeAsync(StateHasChanged);

            //

            _solidsVariantLoader = new VariantLoader(StorageService, new Integrant4.Colorant.Themes.Solids.Theme(),
                Integrant4.Colorant.Themes.Solids.Variants.Normal.ToString());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // await _themeLoader.Load();
                await _defaultVariantLoader.Load();
            }
        }
    }
}