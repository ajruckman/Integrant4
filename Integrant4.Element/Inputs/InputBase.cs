using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract partial class InputBase<T> : IWritableRefreshableInput<T>
    {
        protected readonly IJSRuntime JSRuntime;

        protected ElementReference Reference;
        protected T?               Value;

        internal InputBase(IJSRuntime jsRuntime, SpecSet outerSpec, SpecSet innerSpec)
        {
            JSRuntime = jsRuntime;
            OuterSpec = outerSpec;
            InnerSpec = innerSpec;
            ID        = RandomIDGenerator.Generate();
        }

        internal InputBase(IJSRuntime jsRuntime, UnifiedSpec spec)
        {
            JSRuntime = jsRuntime;
            OuterSpec = spec.ToSpec();
            ID        = RandomIDGenerator.Generate();
        }

        internal InputBase(IJSRuntime jsRuntime, DualSpec spec)
        {
            JSRuntime = jsRuntime;
            OuterSpec = spec.ToOuterSpec();
            InnerSpec = spec.ToInnerSpec();
            ID        = RandomIDGenerator.Generate();
        }

        protected WriteOnlyHook? Refresher { get; set; }

        public abstract RenderFragment Renderer();

        public abstract T?                GetValue();
        public abstract Task<T?>          ReadValue();
        public abstract Task              SetValue(T? value, bool invokeOnChange = true);
        public abstract event Action<T?>? OnChange;

        public void Refresh() => Refresher?.Invoke();
    }

    public partial class InputBase<T>
    {
        internal readonly SpecSet? InnerSpec;
        internal readonly SpecSet? OuterSpec;
        internal readonly string   ID;
    }
}