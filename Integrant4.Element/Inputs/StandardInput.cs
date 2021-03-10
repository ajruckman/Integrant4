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
            : base(jsRuntime, spec, classes)
        {
        }

        public override async Task<T?> GetValue()
        {
            var v = await JSRuntime.InvokeAsync<string>("window.I4.Element.Inputs.GetValue", Reference);
            Value = Nullify(Deserialize(v));
            return Value;
        }

        public override async Task SetValue(T? value)
        {
            Value = Nullify(value);
            await JSRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetValue", Reference, Serialize(Value));
            OnChange?.Invoke(Value);
        }

        public abstract override RenderFragment Renderer();

        public override event Action<T?>? OnChange;

        protected void InvokeOnChange(T value) => OnChange?.Invoke(Nullify(value));

        protected abstract string Serialize(T?        v);
        protected abstract T      Deserialize(string? v);
        protected abstract T      Nullify(T?          v);
    }
}