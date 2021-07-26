using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.Headers
{
    public class SecondaryHeader : IConstruct
    {
        private readonly ContentRef      _leftElements;
        private readonly ContentRef?     _rightElements;
        private readonly Callbacks.Size? _padding;

        public SecondaryHeader(ContentRef leftElements, ContentRef? rightElements, Callbacks.Size? padding = null)
        {
            _leftElements  = leftElements;
            _rightElements = rightElements;
            _padding       = padding;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct I4E-Construct-SecondaryHeader");
            builder.AddAttribute(++seq, "style", _padding == null
                ? null
                : $"padding: {_padding.Invoke().Serialize()};");

            //

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-SecondaryHeader-LeftElements");
            foreach (IRenderable renderable in _leftElements.GetAll())
            {
                builder.AddContent(++seq, renderable.Renderer());
            }

            builder.CloseElement();

            //

            if (_rightElements != null)
            {
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-SecondaryHeader-RightElements");
                foreach (IRenderable renderable in _rightElements.GetAll())
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }

                builder.CloseElement();
            }

            //

            builder.CloseElement();
        };
    }
}