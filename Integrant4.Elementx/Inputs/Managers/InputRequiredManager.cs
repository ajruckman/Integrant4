using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs.Managers
{
    public class InputRequiredManager
    {
        private readonly IJSRuntime _jsRuntime;
        private          bool       _required;

        public InputRequiredManager(IJSRuntime jsRuntime, bool required)
        {
            _jsRuntime = jsRuntime;
            _required  = required;
        }

        public bool IsRequired() => _required;

        public async Task Require(ElementReference   reference) => await SetRequired(_jsRuntime, reference, true);
        public async Task Unrequire(ElementReference reference) => await SetRequired(_jsRuntime, reference, false);

        private async Task SetRequired(IJSRuntime jsRuntime, ElementReference reference, bool required)
        {
            _required = required;
            await jsRuntime.InvokeVoidAsync("window.Integrant.Dominant.SetRequired", reference, required);
        }
    }
}