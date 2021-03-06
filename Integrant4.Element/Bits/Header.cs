using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Header : IBit
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.Size? Padding { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Padding = Padding,
            };
        }
    }

    public partial class Header
    {
        private readonly ContentRef      _content;
        private readonly Style           _style;
        private readonly Callbacks.Size? _padding;

        public Header
        (
            ContentRef content,
            Style      style = Style.Primary,
            Spec?      spec  = null
        )
        {
            _content = content;
            _style   = style;
            _padding = spec?.Padding;
        }
    }

    public partial class Header
    {
        public RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    $"I4E-Bit I4E-Bit-Header " +
                    $"I4E-Bit-Header--{_style}");

                ++seq;
                if (_padding != null)
                {
                    Size v = _padding.Invoke();
                    builder.AddAttribute(seq, "style",
                        $"padding: {v.Top.Serialize()} {v.Right.Serialize()} {v.Bottom.Serialize()} {v.Left.Serialize()};");
                }

                foreach (IRenderable renderable in _content.GetAll())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Header-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }

    public partial class Header
    {
        public enum Style
        {
            Primary, Secondary,
        }
    }
}