using System;
using System.Collections.Generic;
using Integrant4.API;
using Integrant4.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public class Button : IBit
    {
        private readonly Callbacks.Content?  _content;
        private readonly Callbacks.Contents? _contents;

        public Button(Callbacks.Contents contents)
        {
            _contents = contents;
        }

        public Button(Callbacks.Content content)
        {
            _content = content;
        }

        public RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "button");
                builder.AddAttribute(++seq, "class", "I4E.Button");

                if (_content != null)
                {
                    builder.AddContent(++seq, _content.Invoke().Fragment);
                }
                else if (_contents != null)
                {
                    foreach (Content content in _contents.Invoke())
                    {
                        builder.AddContent(++seq, content.Fragment);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                builder.CloseElement();
            }
        }
    }
}