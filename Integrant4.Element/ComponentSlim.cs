namespace Integrant4.Element
{
    // internal abstract class ComponentSlimParent
    // {
    //     internal abstract void SetStateHasChanged(Action stateHasChanged);
    //     internal abstract void BuildRenderTree(RenderTreeBuilder builder);
    // }
    //
    // internal sealed class ComponentSlim<T> : IComponent where T : class
    // {
    //     private RenderHandle _renderHandle;
    //
    //     [Parameter] public T Parent { get; set; } = null!;
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
    //         _renderHandle.Render(Parent.BuildRenderTree);
    //
    //         Parent.SetStateHasChanged(() => _renderHandle.Render(Parent.BuildRenderTree));
    //
    //         return Task.CompletedTask;
    //     }
    // }
}