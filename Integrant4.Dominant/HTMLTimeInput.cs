using System;
using System.Globalization;
using System.Threading.Tasks;
using Integrant4.Dominant.Contracts;
using Integrant4.Dominant.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Dominant
{
    public partial class HTMLTimeInput : InputBase<DateTime?>, IHTMLInput<DateTime?>, IInputRequirable,
        IInputDisableable
    {
        public HTMLTimeInput(IJSRuntime jsRuntime, DateTime? value, bool disabled, bool required)
            : base(jsRuntime, value)
        {
            _inputDisabledManager = new InputDisabledManager(jsRuntime, disabled);
            _inputRequiredManager = new InputRequiredManager(jsRuntime, required);
        }

        public RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "time");
                builder.AddAttribute(++seq, "value", Value?.ToString("HH:mm"));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        private void Change(ChangeEventArgs args) => InvokeOnChange(Parse(args.Value?.ToString()));

        protected override DateTime? Parse(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : DateTime.ParseExact(v, v.Split(':').Length == 2
                        ? "HH:mm"
                        : "HH:mm:ss",
                    new DateTimeFormatInfo());

        protected override string Encode(DateTime? v) => v?.ToString("HH:mm") ?? "";

        protected override DateTime? NullCheck(DateTime? v) =>
            v == null || v == DateTime.MinValue
                ? null
                : v.Value;
    }

    public partial class HTMLTimeInput
    {
        private readonly InputDisabledManager _inputDisabledManager;

        public       Task<bool> IsDisabled() => Task.FromResult(_inputDisabledManager.IsDisabled());
        public async Task       Disable()    => await _inputDisabledManager.Disable(Reference);
        public async Task       Enable()     => await _inputDisabledManager.Enable(Reference);
    }

    public partial class HTMLTimeInput
    {
        private readonly InputRequiredManager _inputRequiredManager;

        public       Task<bool> IsRequired() => Task.FromResult(_inputRequiredManager.IsRequired());
        public async Task       Require()    => await _inputRequiredManager.Require(Reference);
        public async Task       Unrequire()  => await _inputRequiredManager.Unrequire(Reference);
    }
}