using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Inputs.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Superset.Web.Markup;

namespace Integrant4.Element.Inputs
{
    public partial class CheckboxInput : IInput<bool>, IInputDisableable, IInputRequirable
    {
        private readonly IJSRuntime _jsRuntime;

        private bool             _value;
        private ElementReference _reference;

        public CheckboxInput
        (
            IJSRuntime               jsRuntime,  bool value, 
            Callbacks.Callback<bool> isDisabled, Callbacks.Callback<bool> isRequired
        )
        {
            _jsRuntime = jsRuntime;
            _value     = value;

            _inputDisabledManager = new InputDisabledManager(jsRuntime, isDisabled.Invoke());
            _inputRequiredManager = new InputRequiredManager(jsRuntime, isRequired.Invoke());
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
                builder.AddAttribute(++seq, "disabled", _inputDisabledManager.IsDisabled());
                builder.AddAttribute(++seq, "required", _inputRequiredManager.IsRequired());

                builder.AddElementReferenceCapture(++seq, r => _reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        public async Task<bool> GetValue()
        {
            var v = await _jsRuntime.InvokeAsync<bool>("window.I4.Element.Inputs.GetChecked", _reference);
            throw new NotImplementedException();
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

    public partial class CheckboxInput
    {
        private readonly InputDisabledManager _inputDisabledManager;

        public       Task<bool> IsDisabled() => Task.FromResult(_inputDisabledManager.IsDisabled());
        public async Task       Disable()    => await _inputDisabledManager.Disable(_reference);
        public async Task       Enable()     => await _inputDisabledManager.Enable(_reference);
    }

    public partial class CheckboxInput
    {
        private readonly InputRequiredManager _inputRequiredManager;

        public       Task<bool> IsRequired() => Task.FromResult(_inputRequiredManager.IsRequired());
        public async Task       Require()    => await _inputRequiredManager.Require(_reference);
        public async Task       Unrequire()  => await _inputRequiredManager.Unrequire(_reference);
    }
}