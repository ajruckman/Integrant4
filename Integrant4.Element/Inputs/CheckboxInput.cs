using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class CheckboxInput : IInput<bool>
    {
        private readonly IJSRuntime                _jsRuntime;
        private readonly Callbacks.Callback<bool>? _isDisabled;
        private readonly Callbacks.Callback<bool>? _isRequired;

        private bool             _value;
        private ElementReference _reference;

        public CheckboxInput
        (
            IJSRuntime jsRuntime,
            bool       value, Callbacks.Callback<bool>? isDisabled = null, Callbacks.Callback<bool>? isRequired = null
        )
        {
            _jsRuntime  = jsRuntime;
            _value      = value;
            _isDisabled = isDisabled;
            _isRequired = isRequired;
        }

        public RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "checkbox");
                builder.AddAttribute(++seq, "checked", _value);
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", _isDisabled?.Invoke());
                builder.AddAttribute(++seq, "required", _isRequired?.Invoke());

                builder.AddElementReferenceCapture(++seq, r => _reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        public async Task<bool> GetValue()
        {
            return await _jsRuntime.InvokeAsync<bool>("window.I4.Element.Inputs.GetChecked", _reference);
        }

        public async Task SetValue(bool value)
        {
            _value = value;
            await _jsRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetChecked", _reference, _value);
            OnChange?.Invoke(_value);
        }

        public event Action<bool>? OnChange;

        private void Change(ChangeEventArgs args) => OnChange?.Invoke(Deserialize(args.Value?.ToString()));

        private bool Deserialize(string? v) =>
            v switch
            {
                "False" => false,
                "True"  => true,
                _       => throw new ArgumentOutOfRangeException(),
            };
    }
}