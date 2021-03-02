using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs.Managers
{
    public class InputPlaceholderManager
    {
        private readonly IJSRuntime _jsRuntime;
        private          string?    _placeholder;

        public InputPlaceholderManager(IJSRuntime jsRuntime, string? placeholder)
        {
            _jsRuntime   = jsRuntime;
            _placeholder = placeholder;
        }

        public string? GetPlaceholder() => _placeholder;

        public async Task SetPlaceholder(ElementReference reference, string? placeholder)
        {
            _placeholder = placeholder == "" ? null : placeholder;
            await _jsRuntime.InvokeVoidAsync("window.Integrant.Dominant.SetPlaceholder", reference, placeholder ?? "");
        }
    }
}