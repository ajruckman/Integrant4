using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Fundament
{
    public class HookObserver : IComponent
    {
        private RenderHandle _renderHandle;

        [Parameter] public RenderFragment ChildContent { get; set; } = null!;
        [Parameter] public Hook           Hook         { get; set; } = null!;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Hook == null)
            {
                throw new Exception("Hook was not passed to HookObserver component.");
            }

            _renderHandle.Render(ChildContent);

            Hook.Event += () => _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(ChildContent));

            return Task.CompletedTask;
        }
    }
}