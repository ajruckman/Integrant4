using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class TextInput : InputBase<string?>
    {
        private readonly Callbacks.Callback<bool>?   _isDisabled;
        private readonly Callbacks.Callback<bool>?   _isRequired;
        private readonly Callbacks.Callback<string>? _placeholder;

        public TextInput
        (
            IJSRuntime                  jsRuntime,
            string?                     value,
            Callbacks.Callback<bool>?   isDisabled  = null,
            Callbacks.Callback<bool>?   isRequired  = null,
            Callbacks.Callback<string>? placeholder = null
        )
            : base(jsRuntime)
        {
            _isDisabled  = isDisabled;
            _isRequired  = isRequired;
            _placeholder = placeholder;
            
            Value = Nullify(value);
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
                builder.AddAttribute(++seq, "disabled", _isDisabled?.Invoke());
                builder.AddAttribute(++seq, "required", _isRequired?.Invoke());
                builder.AddAttribute(++seq, "placeholder", _placeholder?.Invoke());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override        string  Serialize(string?   v) => v ?? "";
        protected override        string? Deserialize(string? v) => string.IsNullOrEmpty(v) ? null : v;
        protected sealed override string? Nullify(string?     v) => string.IsNullOrEmpty(v) ? null : v;

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}