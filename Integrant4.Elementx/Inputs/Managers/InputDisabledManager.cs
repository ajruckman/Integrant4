using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs.Managers
{
    // public class InputDisabledManager
    // {
    //     private readonly IJSRuntime               _jsRuntime;
    //     private          Callbacks.Callback<bool> _disabled;
    //
    //     public InputDisabledManager(IJSRuntime jsRuntime, Callbacks.Callback<bool> disabled)
    //     {
    //         _jsRuntime = jsRuntime;
    //         _disabled  = disabled;
    //     }
    //
    //     public bool IsDisabled() => _disabled.Invoke();
    //
    //     public async Task Disable(ElementReference reference) => await SetDisabled(reference, true);
    //     public async Task Enable(ElementReference  reference) => await SetDisabled(reference, false);
    //
    //     private async Task SetDisabled(ElementReference reference, bool disabled)
    //     {
    //         _disabled = disabled;
    //         await _jsRuntime.InvokeVoidAsync("window.Integrant.Dominant.SetDisabled", reference, disabled);
    //     }
    // }
}