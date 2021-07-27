using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract partial class NumberInput<T> : StandardInput<T>
    {
        internal NumberInput(IJSRuntime jsRuntime, SpecSet? outerSpec, SpecSet? innerSpec)
            : base(jsRuntime, outerSpec, innerSpec) { }

        internal NumberInput(IJSRuntime jsRuntime, UnifiedSpec? spec)
            : base(jsRuntime, spec) { }

        internal NumberInput(IJSRuntime jsRuntime, DualSpec? spec)
            : base(jsRuntime, spec) { }

        protected override string Serialize(T? v) => v?.ToString() ?? "";

        protected void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}