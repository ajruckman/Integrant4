using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public partial class LocalModal : IRefreshableBit
    {
        private readonly ContentRef      _content;
        private readonly InnerDirection? _innerDirection;

        private bool           _visible;
        private WriteOnlyHook? _refresher;

        public LocalModal
        (
            ContentRef     content,
            InnerDirection innerDirection = InnerDirection.Row,
            ReadOnlyHook?  hook           = null
        )
        {
            _content        = content;
            _innerDirection = innerDirection;

            if (hook != null)
                hook.Event += Refresh;
        }
    }

    public partial class LocalModal
    {
        public RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Bit-LocalModal I4E-Bit-LocalModal" + (_visible ? "--Visible" : "--Hidden"));

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Bit-LocalModal-Inner I4E-Bit-LocalModal-Inner--" + _innerDirection);

            foreach (IRenderable renderable in _content.GetAll())
            {
                builder.AddContent(++seq, renderable.Renderer());
            }

            builder.CloseElement();

            builder.CloseElement();
        }, v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        public void Show()
        {
            _visible = true;
            Refresh();
        }

        public void Hide()
        {
            _visible = false;
            Refresh();
        }
    }

    public partial class LocalModal
    {
        public enum InnerDirection
        {
            Row, Column,
        }
    }
}