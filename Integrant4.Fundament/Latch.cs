using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    public sealed partial class Latch : IComponent, IHandleAfterRender
    {
        private RenderHandle _renderHandle;

        private bool _hasCalledOnAfterRender;

        [Parameter] public RenderFragment ChildContent { get; set; } = null!;
        [Parameter] public ReadOnlyHook   Hook         { get; set; } = null!;

        [Parameter] public Func<bool, Task>? AfterRender { get; set; }

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Hook == null)
            {
                throw new Exception("Hook was not passed to Latch component.");
            }

            _renderHandle.Render(ChildContent);

            Hook.Event += async () =>
                await _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(ChildContent));

            return Task.CompletedTask;
        }

        public async Task OnAfterRenderAsync()
        {
            if (AfterRender == null) return;

            bool firstRender = !_hasCalledOnAfterRender;
            _hasCalledOnAfterRender = true;

            await AfterRender.Invoke(firstRender);
        }
    }

    public sealed partial class Latch
    {
        public static RenderFragment Create
        (
            RenderFragment        content,
            Action<WriteOnlyHook> hookSetter,
            Func<bool, Task>?     afterRender = null
        )
        {
            Hook hook = new();

            void Fragment(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Latch>(0);
                builder.AddAttribute(1, "Hook",         hook.AsReadOnly());
                builder.AddAttribute(2, "ChildContent", content);
                builder.AddAttribute(2, "AfterRender",  afterRender);
                builder.CloseComponent();
            }

            hookSetter.Invoke(hook);

            return Fragment;
        }
    }
}