using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    public sealed class RefreshWrapper : IComponent
    {
        private RenderHandle _renderHandle;

        [Parameter] public RenderFragment     ChildContent { get; set; } = null!;
        [Parameter] public Action<Func<Task>> SetRefresher { get; set; } = null!;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            _renderHandle.Render(ChildContent);

            SetRefresher.Invoke
            (
                async () => await _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(ChildContent))
            );

            return Task.CompletedTask;
        }

        public static RenderFragment Create
        (
            RenderFragment     content,
            Action<Func<Task>> refresherSetter
        )
        {
            void Fragment(RenderTreeBuilder builder)
            {
                builder.OpenComponent<RefreshWrapper>(0);
                builder.AddAttribute(1, "SetRefresher", refresherSetter);
                builder.AddAttribute(2, "ChildContent", content);
                builder.CloseComponent();
            }

            return Fragment;
        }
    }
}