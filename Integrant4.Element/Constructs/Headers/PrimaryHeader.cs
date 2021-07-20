using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.Headers
{
    public class PrimaryHeader : IConstruct
    {
        private readonly ContentRef  _leftElements;
        private readonly ContentRef? _rightElements;

        public PrimaryHeader(ContentRef leftElements, ContentRef? rightElements)
        {
            _leftElements  = leftElements;
            _rightElements = rightElements;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct I4E-Construct-PrimaryHeader");

            //

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-PrimaryHeader-LeftElements");
            foreach (IRenderable renderable in _leftElements.GetAll())
            {
                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "I4E-Construct-PrimaryHeader-LeftElement");
                builder.AddContent(++seq, renderable.Renderer());
                builder.CloseElement();
            }

            builder.CloseElement();

            //

            if (_rightElements != null)
            {
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-PrimaryHeader-RightElements");
                foreach (IRenderable renderable in _rightElements.GetAll())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-PrimaryHeader-RightElement");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            //

            builder.CloseElement();
        };
    }
}