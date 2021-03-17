using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class RenderableArray : IRenderable
    {
        private readonly IRenderable[] _values;

        public RenderableArray(IRenderable[] values)
        {
            _values = values;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            foreach (IRenderable renderable in _values)
            {
                builder.AddContent(++seq, renderable.Renderer());
            }
        };
    }
}