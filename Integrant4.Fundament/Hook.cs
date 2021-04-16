using System;

// ReSharper disable InconsistentNaming

namespace Integrant4.Fundament
{
    public class Hook
    {
        public event Action? Event;

        public void Invoke() => Event?.Invoke();
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