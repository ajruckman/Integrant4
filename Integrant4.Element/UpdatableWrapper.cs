using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element
{
    public sealed class UpdatableWrapper : ComponentBase
    {
        public sealed class RefreshSignaler
        {
            public event Action? OnSignal;
            public void          Signal() => OnSignal?.Invoke();
        }

        [Parameter] public RefreshSignaler Signaler     { get; set; } = null!;
        [Parameter] public RenderFragment  ChildContent { get; set; } = null!;

        protected override void OnParametersSet() => 
            Signaler.OnSignal += () => InvokeAsync(StateHasChanged);

        protected override void BuildRenderTree(RenderTreeBuilder builder) => 
            builder.AddContent(0, ChildContent);
    }
}