using Microsoft.AspNetCore.Components;

namespace Integrant4.API
{
    public interface IRenderable
    {
        public RenderFragment Renderer();
    }
}