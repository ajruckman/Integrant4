using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public static class Interop
    {
        public static async Task CreateTooltips(IJSRuntime jsRuntime, CancellationToken token, string id)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.InitTooltip", token, id);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        public static async Task HighlightPageLink
            (IJSRuntime jsRuntime, CancellationToken token, string id, bool highlighted)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("I4.Element.HighlightPageLink", token, id, highlighted);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }
    }
}