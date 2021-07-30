using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public partial class ElementService : IDisposable
    {
        public delegate Task Job(IJSRuntime jsRuntime, CancellationToken token);

        private readonly IJSRuntime              _jsRuntime;
        private readonly NavigationManager       _navigationManager;
        private readonly ConcurrentQueue<Job>    _jobs = new();
        private readonly CancellationTokenSource _cts  = new();
        private readonly Guid                    _guid = Guid.NewGuid();

        private readonly Dictionary<string, string> _links     = new();
        private readonly object                     _linksLock = new();

        public ElementService(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _jsRuntime         = jsRuntime;
            _navigationManager = navigationManager;
        }

        public IJSRuntime        JSRuntime         => _jsRuntime;
        public CancellationToken CancellationToken => _cts.Token;

        public void Dispose()
        {
            Console.WriteLine("ElementService disposed");
            _cts.Cancel();
            _cts.Dispose();
        }

        internal void RegisterLink(string id, string href)
        {
            lock (_linksLock)
            {
                _links[id] = href;
            }
        }

        private async Task ProcessLinks()
        {
            string currentURL = "/" + _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

            KeyValuePair<string, string>[] links;

            lock (_linksLock)
            {
                links = _links.ToArray();
                _links.Clear();
            }

            foreach ((string id, string href) in links)
            {
                await Interop.HighlightHeaderLink(_jsRuntime, CancellationToken, id, href == currentURL);
            }
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

            await ProcessLinks();
        }

        public async Task JSInvokeVoidAsync(string identifier, params object[] args)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(identifier, CancellationToken, args);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }
    }

    public partial class ElementService
    {
        public enum InteractionType
        {
            Click,
        }

        private Action<Interaction>? _interactionLogger;

        internal void LogInteraction<TData>
        (
            InteractionType type,
            string          elementType,
            string?         loggingID,
            TData           data
        ) where TData : notnull
        {
            if (loggingID == null) return;

            _interactionLogger?.Invoke(new Interaction(type, elementType, loggingID, data));
        }

        public void UseInteractionLogger(Action<Interaction> logger)
        {
            _interactionLogger = logger;
        }

        public class Interaction
        {
            public Interaction(InteractionType type, string elementType, string loggingID, object data)
            {
                Type        = type;
                ElementType = elementType;
                LoggingID   = loggingID;
                Data        = data;
            }

            public InteractionType Type        { get; }
            public string          ElementType { get; }
            public string          LoggingID   { get; }
            public object          Data        { get; }
        }
    }
}