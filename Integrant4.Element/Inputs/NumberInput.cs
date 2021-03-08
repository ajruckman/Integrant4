using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract partial class NumberInput<T>
    {
    }

    public partial class NumberInput<T> : StandardInput<T>
    {
        internal NumberInput
        (
            IJSRuntime jsRuntime,
            ClassSet   classSet,
            BaseSpec?  spec = null
        )
            : base(jsRuntime, spec, classSet)
        {
        }

        protected override string Serialize(T? v) => v?.ToString() ?? "";

        protected void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}