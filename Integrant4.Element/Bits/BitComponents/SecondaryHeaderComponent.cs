using Integrant4.Element.Constructs.Headers;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits.BitComponents
{
    public class SecondaryHeaderComponent : ComponentBase
    {
        private SecondaryHeader? _header;

        [Parameter] public RenderFragment ChildContent { get; set; } = null!;

        protected override void OnParametersSet()
        {
            _header = new SecondaryHeader(ContentRef.Dynamic(() => ChildContent), null);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _header?.Renderer());
        }
    }
}