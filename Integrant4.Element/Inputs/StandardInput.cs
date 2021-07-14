using System;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract class StandardInput<T> : InputBase<T>
    {
        internal StandardInput(IJSRuntime jsRuntime, BaseSpec? spec, ClassSet classes)
            : base(jsRuntime, spec, classes) { }

        public override T? GetValue() => Value;

        public override async Task<T?> ReadValue() =>
            Value = Nullify(Deserialize(
                await JSRuntime.InvokeAsync<string>("window.I4.Element.Inputs.GetValue", Reference)));

        public override async Task SetValue(T? value, bool invokeOnChange = true)
        {
            Value = Nullify(value);
            await JSRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetValue", Reference, Serialize(Value));

            if (invokeOnChange) OnChange?.Invoke(Value);
        }

        public abstract override RenderFragment Renderer();

        public override event Action<T?>? OnChange;

        protected void InvokeOnChange(T value)
        {
            Value = value;
            OnChange?.Invoke(Nullify(value));
        }

        protected abstract string Serialize(T?        v);
        protected abstract T      Deserialize(string? v);
        protected abstract T      Nullify(T?          v);
    }
}