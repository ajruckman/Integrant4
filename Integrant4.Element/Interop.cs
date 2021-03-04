using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public static class Interop
    {
        public static async Task CreateBitTooltips(IJSRuntime jsRuntime, string id)
        {
            await jsRuntime.InvokeVoidAsync("I4.Element.CreateBitTooltips", id);
        }
    }
}