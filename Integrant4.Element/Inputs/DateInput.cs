using System;
using System.Globalization;
using System.Threading.Tasks;
using Integrant4.Element.Inputs.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class DateInput : InputBase<DateTime?>, IInputRequirable, IInputDisableable
    {
        public DateInput(IJSRuntime jsRuntime, DateTime? value, bool disabled, bool required)
            : base(jsRuntime, value)
        {
            _inputDisabledManager = new InputDisabledManager(jsRuntime, disabled);
            _inputRequiredManager = new InputRequiredManager(jsRuntime, required);
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "date");
                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", _inputDisabledManager.IsDisabled());
                builder.AddAttribute(++seq, "required", _inputRequiredManager.IsRequired());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));

        protected override string Serialize(DateTime? v) => v?.ToString("yyyy-MM-dd") ?? "";

        protected override DateTime? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : DateTime.ParseExact(v, "yyyy-MM-dd", new DateTimeFormatInfo());

        protected override DateTime? Nullify(DateTime? v) =>
            v == null || v.Value == DateTime.MinValue
                ? null
                : v.Value;
    }

    public partial class DateInput
    {
        private readonly InputDisabledManager _inputDisabledManager;

        public       Task<bool> IsDisabled() => Task.FromResult(_inputDisabledManager.IsDisabled());
        public async Task       Disable()    => await _inputDisabledManager.Disable(Reference);
        public async Task       Enable()     => await _inputDisabledManager.Enable(Reference);
    }

    public partial class DateInput
    {
        private readonly InputRequiredManager _inputRequiredManager;

        public       Task<bool> IsRequired() => Task.FromResult(_inputRequiredManager.IsRequired());
        public async Task       Require()    => await _inputRequiredManager.Require(Reference);
        public async Task       Unrequire()  => await _inputRequiredManager.Unrequire(Reference);
    }
}