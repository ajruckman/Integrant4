using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public partial class LocalModal : IRefreshableBit
    {
        private readonly DynamicContents _contents;
        private readonly InnerDirection? _innerDirection;

        private bool           _show;
        private WriteOnlyHook? _refresher;

        public LocalModal
        (
            DynamicContents contents,
            InnerDirection  innerDirection = InnerDirection.Row,
            ReadOnlyHook?   hook           = null
        )
        {
            _contents       = contents;
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
                "I4E-Bit-LocalModal I4E-Bit-LocalModal" + (_show ? "--Show" : "--Hide"));

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Bit-LocalModal-Inner I4E-Bit-LocalModal-Inner--" + _innerDirection);

            foreach (IRenderable renderable in _contents.Invoke())
            {
                builder.AddContent(++seq, renderable.Renderer());
            }

            builder.CloseElement();

            builder.CloseElement();
        }, v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        public void Show()
        {
            _show = true;
            Refresh();
        }

        public void Hide()
        {
            _show = false;
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