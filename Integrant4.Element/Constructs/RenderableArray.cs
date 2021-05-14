using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class RenderableArray : IRenderable
    {
        private readonly IRenderable[]        _values;
        private readonly Callbacks.IsVisible? _isVisible;

        public RenderableArray(IRenderable[] values, Callbacks.IsVisible? isVisible = null)
        {
            _values    = values;
            _isVisible = isVisible;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            if (_isVisible == null || _isVisible?.Invoke() == true)
            {
                foreach (IRenderable renderable in _values)
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }
            }
        };
    }
}