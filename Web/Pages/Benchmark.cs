using System.Threading;
using System.Threading.Tasks;

namespace Web.Pages
{
    public partial class Benchmark
    {
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                Task.Run(() =>
                {
                    Thread.Sleep(100);
                    InvokeAsync(StateHasChanged);
                });
        }
    }
}