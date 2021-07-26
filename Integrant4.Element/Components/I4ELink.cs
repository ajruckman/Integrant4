using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Components
{
    public class I4ELink : ComponentBase
    {
        private Link? _link;

        [Parameter] public string         HREF          { get; set; } = null!;
        [Parameter] public bool           IsAccented    { get; set; }
        [Parameter] public bool           IsHighlighted { get; set; }
        [Parameter] public RenderFragment ChildContent  { get; set; } = null!;

        protected override void OnParametersSet()
        {
            _link = new Link(ChildContent.AsStatic(), new Link.Spec(() => HREF)
            {
                IsAccented    = () => IsAccented,
                IsHighlighted = () => IsHighlighted,
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _link?.Renderer());
        }
    }
}