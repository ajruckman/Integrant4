using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Inputs.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TextInput : InputBase<string?>, IInputRequirable, IInputDisableable,
        IInputWithPlaceholder
    {
        public TextInput(IJSRuntime jsRuntime, string? value, bool disabled, bool required, string? placeholder)
            : base(jsRuntime, value)

        {
            _inputDisabledManager    = new InputDisabledManager(jsRuntime, disabled);
            _inputRequiredManager    = new InputRequiredManager(jsRuntime, required);
            _inputPlaceholderManager = new InputPlaceholderManager(jsRuntime, placeholder);
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                Console.WriteLine("RENDER");
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "text");
                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", _inputDisabledManager.IsDisabled());
                builder.AddAttribute(++seq, "required", _inputRequiredManager.IsRequired());
                builder.AddAttribute(++seq, "placeholder", _inputPlaceholderManager.GetPlaceholder());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override string  Serialize(string?   v) => v ?? "";
        protected override string? Deserialize(string? v) => string.IsNullOrEmpty(v) ? null : v;
        protected override string? Nullify(string?     v) => string.IsNullOrEmpty(v) ? null : v;

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }

    public partial class TextInput
    {
        private readonly InputDisabledManager _inputDisabledManager;

        public       Task<bool> IsDisabled() => Task.FromResult(_inputDisabledManager.IsDisabled());
        public async Task       Disable()    => await _inputDisabledManager.Disable(Reference);
        public async Task       Enable()     => await _inputDisabledManager.Enable(Reference);
    }

    public partial class TextInput
    {
        private readonly InputRequiredManager _inputRequiredManager;

        public       Task<bool> IsRequired() => Task.FromResult(_inputRequiredManager.IsRequired());
        public async Task       Require()    => await _inputRequiredManager.Require(Reference);
        public async Task       Unrequire()  => await _inputRequiredManager.Unrequire(Reference);
    }

    public partial class TextInput
    {
        private readonly InputPlaceholderManager _inputPlaceholderManager;

        public Task<string?> GetPlaceholder() => Task.FromResult(_inputPlaceholderManager.GetPlaceholder());

        public async Task SetPlaceholder(string? placeholder) =>
            await _inputPlaceholderManager.SetPlaceholder(Reference, placeholder);
    }
}