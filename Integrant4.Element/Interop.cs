using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public static class Interop
    {
        public static async Task CreateTooltips(IJSRuntime jsRuntime, string id)
        {
            await jsRuntime.InvokeVoidAsync("I4.Element.InitTooltip", id);
        }

        public static async Task HighlightPageLink(IJSRuntime jsRuntime, string id, bool highlighted)
        {
            await jsRuntime.InvokeVoidAsync("I4.Element.HighlightPageLink", id, highlighted);
        }
    }
}