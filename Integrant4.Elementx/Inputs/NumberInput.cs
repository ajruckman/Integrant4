using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract class NumberInput<T> : InputBase<T>
    {
        protected readonly Callbacks.Callback<bool>? IsDisabled;
        protected readonly Callbacks.Callback<bool>? IsRequired;
        protected readonly Callbacks.Callback<bool>  Consider0Null;

        protected NumberInput
        (
            IJSRuntime                jsRuntime,
            T                         value,
            Callbacks.Callback<bool>? isDisabled,
            Callbacks.Callback<bool>? isRequired,
            Callbacks.Callback<bool>? consider0Null
        ) : base(jsRuntime)
        {
            IsDisabled    = isDisabled;
            IsRequired    = isRequired;
            Consider0Null = consider0Null ?? (() => false);
        }

        protected override string Serialize(T? v) => v?.ToString() ?? "";

        protected void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}