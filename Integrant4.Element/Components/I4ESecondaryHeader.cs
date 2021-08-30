using Integrant4.Element.Constructs.Headers;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Components
{
    public class I4ESecondaryHeader : ComponentBase
    {
        private SecondaryHeader? _header;

        [Parameter] public RenderFragment ChildContent { get; set; } = null!;
        [Parameter] public bool?          BorderTop    { get; set; }

        protected override void OnParametersSet()
        {
            _header = new SecondaryHeader(ContentRef.Dynamic(() => ChildContent), null,
                borderTop: BorderTop == null ? null : () => BorderTop.Value);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _header?.Renderer());
        }
    }
}