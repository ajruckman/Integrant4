using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Components
{
    public class I4EExpander : ComponentBase
    {
        [Parameter] public RenderFragment ExpandContent   { get; set; } = null!;
        [Parameter] public RenderFragment ContractContent { get; set; } = null!;
        [Parameter] public RenderFragment Content         { get; set; } = null!;

        private Expander _expander = null!;

        protected override void OnInitialized()
        {
            _expander = new Expander
            (
                ExpandContent.AsStatic(),
                ContractContent.AsStatic()
            );

            _expander.Hook.Event += () => InvokeAsync(StateHasChanged);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _expander.Renderer());

            if (_expander.Expanded)
                builder.AddContent(1, Content);
        }
    }
}