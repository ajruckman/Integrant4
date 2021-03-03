using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class IntegerInput : NumberInput<int?>
    {
        public IntegerInput
        (
            IJSRuntime                jsRuntime,
            int?                      value,
            Callbacks.Callback<bool>? isDisabled    = null,
            Callbacks.Callback<bool>? isRequired    = null,
            Callbacks.Callback<bool>? consider0Null = null
        )
            : base(jsRuntime, value, isDisabled, isRequired, consider0Null)
        {
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

                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", IsDisabled?.Invoke());
                builder.AddAttribute(++seq, "required", IsRequired?.Invoke());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override int? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : int.Parse(v);

        protected sealed override int? Nullify(int? v) =>
            v == null
                ? null
                : Consider0Null.Invoke() && v == 0
                    ? null
                    : v;
    }
}