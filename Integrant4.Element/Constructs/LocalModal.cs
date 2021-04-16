using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public partial class LocalModal : IRefreshableConstruct
    {
        private readonly Callbacks.BitContents _contents;
        private readonly InnerDirection?       _innerDirection;

        private bool  _show;
        private Hook? _refresher;

        public LocalModal
        (
            Callbacks.BitContents contents,
            InnerDirection        innerDirection = InnerDirection.Row,
            Hook?                 hook           = null
        )
        {
            _contents       = contents;
            _innerDirection = innerDirection;

            if (hook != null)
                hook.Event += async () => await Refresh();
        }
    }

    public partial class LocalModal
    {
        public RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Construct-LocalModal I4E-Construct-LocalModal" + (_show ? "--Show" : "--Hide"));

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Construct-LocalModal-Inner I4E-Construct-LocalModal-Inner--" + _innerDirection);

            foreach (IRenderable renderable in _contents.Invoke())
            {
                builder.AddContent(++seq, renderable.Renderer());
            }

            builder.CloseElement();

            builder.CloseElement();
        }, v => _refresher = v);

        public async Task Refresh() => _refresher?.Invoke();

        public async Task Show()
        {
            _show = true;
            await Refresh();
        }

        public async Task Hide()
        {
            _show = false;
            await Refresh();
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