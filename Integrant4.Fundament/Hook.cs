using System;

// ReSharper disable InconsistentNaming

namespace Integrant4.Fundament
{
    public class Hook
    {
        public event Action? Event;

        public void Invoke() => Event?.Invoke();

        public ReadOnlyHook  AsReadOnly()  => new(this);
        public WriteOnlyHook AsWriteOnly() => new(this);
    }

    public class ReadOnlyHook
    {
        internal ReadOnlyHook(Hook hook)
        {
            hook.Event += () => Event?.Invoke();
        }

        public event Action? Event;

        public static implicit operator ReadOnlyHook(Hook hook) => new(hook);
    }

    public class WriteOnlyHook
    {
        private readonly Action Invoker;

        internal WriteOnlyHook(Hook hook)
        {
            Invoker = hook.Invoke;
        }

        public void Invoke() => Invoker.Invoke();

        public static implicit operator WriteOnlyHook(Hook hook) => new(hook);
    }

    // public class AsyncHook
    // {
    //     private readonly List<Func<Task>> Tasks = new();
    //     
    //     public static AsyncHook operator +(AsyncHook hook, Func<Task> callback)
    //     {
    //         hook.Tasks.Add(callback);
    //
    //         return hook;
    //     } 
    //     
    //     public static AsyncHook operator -(AsyncHook hook, Func<Task> callback)
    //     {
    //         hook.Tasks.Remove(callback);
    //
    //         return hook;
    //     }
    //
    //     public async Task Invoke()
    //     {
    //         await Task.WhenAll(Tasks.Select(v => v.Invoke()));
    //     }
    // }
}