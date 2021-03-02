using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class DateInput : InputBase<DateTime?>
    {
        private readonly Callbacks.Callback<bool>? _isDisabled;
        private readonly Callbacks.Callback<bool>? _isRequired;

        public DateInput
        (
            IJSRuntime                jsRuntime,
            DateTime?                 value,
            Callbacks.Callback<bool>? isDisabled = null,
            Callbacks.Callback<bool>? isRequired = null
        )
            : base(jsRuntime)
        {
            _isDisabled = isDisabled;
            _isRequired = isRequired;

            Value = Nullify(value);
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
                builder.AddAttribute(++seq, "disabled", _isDisabled?.Invoke());
                builder.AddAttribute(++seq, "required", _isRequired?.Invoke());

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

        protected sealed override DateTime? Nullify(DateTime? v) =>
            v == null || v.Value == DateTime.MinValue
                ? null
                : v.Value;
    }
}