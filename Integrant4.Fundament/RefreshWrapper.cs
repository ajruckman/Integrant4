using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    // public class RefreshWrapper : IComponent
    // {
    //     private RenderHandle _renderHandle;
    //
    //     [Parameter] public RenderFragment     ChildContent { get; set; } = null!;
    //     [Parameter] public Action<Func<Task>> SetRefresher { get; set; } = null!;
    //
    //     public void Attach(RenderHandle renderHandle)
    //     {
    //         _renderHandle = renderHandle;
    //     }
    //
    //     public Task SetParametersAsync(ParameterView parameters)
    //     {
    //         parameters.SetParameterProperties(this);
    //
    //         _renderHandle.Render(ChildContent);
    //
    //         SetRefresher.Invoke
    //         (
    //             async () => await _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(ChildContent))
    //         );
    //
    //         return Task.CompletedTask;
    //     }
    //
    //     public static RenderFragment Create
    //     (
    //         RenderFragment     content,
    //         Action<Func<Task>> refresherSetter
    //     )
    //     {
    //         void Fragment(RenderTreeBuilder builder)
    //         {
    //             builder.OpenComponent<RefreshWrapper>(0);
    //             builder.AddAttribute(1, "SetRefresher", refresherSetter);
    //             builder.AddAttribute(2, "ChildContent", content);
    //             builder.CloseComponent();
    //         }
    //
    //         return Fragment;
    //     }
    // }
    //
    // public class TickingRefreshWrapper : RefreshWrapper, IHandleAfterRender
    // {
    //     [Parameter] public Func<bool, Task>? OnAfterRender { get; set; }
    //
    //     private bool _hasCalledOnAfterRender;
    //
    //     public async Task OnAfterRenderAsync()
    //     {
    //         bool firstRender = !_hasCalledOnAfterRender;
    //         _hasCalledOnAfterRender = true;
    //
    //         if (OnAfterRender != null)
    //             await OnAfterRender.Invoke(firstRender);
    //     }
    //
    //     public static RenderFragment Create
    //     (
    //         RenderFragment     content,
    //         Action<Func<Task>> refresherSetter,
    //         Func<bool, Task>?  onAfterRender = null
    //     )
    //     {
    //         void Fragment(RenderTreeBuilder builder)
    //         {
    //             builder.OpenComponent<TickingRefreshWrapper>(0);
    //             builder.AddAttribute(1, "SetRefresher", refresherSetter);
    //             builder.AddAttribute(2, "ChildContent", content);
    //             builder.AddAttribute(2, "OnAfterRender", onAfterRender);
    //             builder.CloseComponent();
    //         }
    //
    //         return Fragment;
    //     }
    // }
}