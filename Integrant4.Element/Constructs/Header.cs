using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public partial class Header : IRenderable
    {
        private readonly Callbacks.BitContents _contents;

        public Header(Callbacks.BitContents contents)
        {
            _contents = contents;
        }
    }

    public partial class Header
    {
        public RenderFragment Renderer()
        {
            throw new System.NotImplementedException();
        }
    }
}