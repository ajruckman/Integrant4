using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class DecimalInput : NumberInput<decimal?>
    {
        private readonly Callbacks.Callback<string> _step;

        public DecimalInput
        (
            IJSRuntime                  jsRuntime,
            decimal?                    value,
            Callbacks.Callback<bool>?   isDisabled    = null,
            Callbacks.Callback<bool>?   isRequired    = null,
            Callbacks.Callback<bool>?   consider0Null = null,
            Callbacks.Callback<string>? step          = null
        )
            : base(jsRuntime, value, isDisabled, isRequired, consider0Null)
        {
            _step = step ?? (() => "0.1");
            
            Value = Nullify(value);
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                Console.WriteLine("RENDER");
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "number");

                ++seq;
                if (Value != null)
                {
                    builder.AddAttribute(seq, "value", Serialize(Value));
                }
                else
                {
                    JSRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetIndeterminate", true);
                }

                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", IsRequired?.Invoke());
                builder.AddAttribute(++seq, "required", IsDisabled?.Invoke());
                builder.AddAttribute(++seq, "step", _step.Invoke());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override decimal? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : decimal.Parse(v);

        protected sealed override decimal? Nullify(decimal? v) =>
            v == null
                ? null
                : Consider0Null.Invoke() && v == 0
                    ? null
                    : v;
    }
}