using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.API;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract class InputBase<T> : IInput<T>
    {
        protected readonly IJSRuntime JSRuntime;

        protected ElementReference Reference;
        protected T?               Value;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected InputBase(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task<T?> GetValue()
        {
            var v = await JSRuntime.InvokeAsync<string>("window.I4.Element.Inputs.GetValue", Reference);
            Value = Nullify(Deserialize(v));
            return Value;
        }

        public async Task SetValue(T? value)
        {
            Value = Nullify(value);
            await JSRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetValue", Reference, Serialize(Value));
            OnChange?.Invoke(Value);
        }

        public abstract RenderFragment Render();

        public event Action<T?>? OnChange;

        protected void InvokeOnChange(T value) => OnChange?.Invoke(Nullify(value));

        protected abstract string Serialize(T?        v);
        protected abstract T      Deserialize(string? v);
        protected abstract T      Nullify(T?          v);
    }
}