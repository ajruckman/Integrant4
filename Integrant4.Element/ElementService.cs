using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public class ElementService
    {
        public delegate  Task       Job(IJSRuntime jsRuntime);
        private readonly IJSRuntime _jsRuntime;

        private readonly ConcurrentQueue<Job> _jobs = new();

        public ElementService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void AddJob(Job job)
        {
            _jobs.Enqueue(job);
        }

        public async Task ProcessJobs()
        {
            while (_jobs.TryDequeue(out Job? job))
            {
                await job.Invoke(_jsRuntime);
            }
        }
    }
}