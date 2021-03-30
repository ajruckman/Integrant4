using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public class ElementService : IDisposable
    {
        public delegate Task Job(IJSRuntime jsRuntime, CancellationToken token);

        private readonly IJSRuntime              _jsRuntime;
        private readonly ConcurrentQueue<Job>    _jobs = new();
        private readonly CancellationTokenSource _cts  = new();
        private readonly Guid                    _guid = Guid.NewGuid();

        public ElementService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public IJSRuntime        JSRuntime         => _jsRuntime;
        public CancellationToken CancellationToken => _cts.Token;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void AddJob(Job job)
        {
            _jobs.Enqueue(job);
        }

        public async Task ProcessJobs()
        {
            while (_jobs.TryDequeue(out Job? job))
            {
                await job.Invoke(_jsRuntime, CancellationToken);
            }
        }
    }
}