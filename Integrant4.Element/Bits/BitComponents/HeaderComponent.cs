using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits.BitComponents
{
    public class HeaderComponent : ComponentBase
    {
        private Header? _header;

        [Parameter] public Header.Style   Style        { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; } = null!;

        protected override void OnParametersSet()
        {
            _header = new Header(ContentRef.Static(ChildContent), Style);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _header?.Renderer());
        }
    }
}