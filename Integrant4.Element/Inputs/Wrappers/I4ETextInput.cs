using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs.Wrappers
{
    public class I4ETextInput : ComponentBase
    {
        private TextInput? _input;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public Action<string?> OnChange { get; set; } = null!;
        [Parameter] public string?         Value    { get; set; }

        [Parameter] public Callbacks.Callback<bool>?   IsDisabled  { get; set; }
        [Parameter] public Callbacks.Callback<bool>?   IsRequired  { get; set; }
        [Parameter] public Callbacks.Callback<string>? Placeholder { get; set; }

        [Parameter] public InputReference<string?>? Reference { get; set; }

        protected override void OnParametersSet()
        {
            if (OnChange == null)
                throw new ArgumentNullException(nameof(OnChange), "Parameter 'OnChange' is required.");

            // _input          =  new TextInput(JSRuntime, Value, IsDisabled, IsRequired, Placeholder);
            _input.OnChange += v => OnChange.Invoke(v);

            Reference?.Set(_input);
        }

        // protected override void BuildRenderTree(RenderTreeBuilder builder) =>
            // builder.AddContent(0, _input?.Render());
    }
}