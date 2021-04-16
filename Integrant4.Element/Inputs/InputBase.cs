using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract partial class InputBase<T> : IRefreshableInput<T>
    {
        protected readonly IJSRuntime JSRuntime;

        protected ElementReference Reference;
        protected T?               Value;

        internal InputBase(IJSRuntime jsRuntime, BaseSpec? spec, ClassSet classes)
        {
            JSRuntime = jsRuntime;

            BaseSpec    = spec ?? new BaseSpec();
            BaseClasses = classes;
            ID          = RandomIDGenerator.Generate();
        }

        public abstract RenderFragment Renderer();

        public abstract Task<T?>          GetValue();
        public abstract Task              SetValue(T? value);
        public abstract event Action<T?>? OnChange;

        protected Hook? Refresher { get; set; }

        public async Task Refresh() => Refresher?.Invoke();
    }

    public partial class InputBase<T>
    {
        internal readonly BaseSpec BaseSpec;
        internal readonly ClassSet BaseClasses;
        internal readonly string   ID;
    }
}