using System.Collections.Generic;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public class Expander : IRefreshableBit
    {
        private static readonly BootstrapIcon DownIcon = new("caret-down-fill", 16);
        private static readonly BootstrapIcon UpIcon   = new("caret-up-fill", 16);

        private readonly Hook   _hook;
        private readonly Button _button;

        private WriteOnlyHook? _refresher;

        public Expander(Callbacks.BitContent text, Spec? spec = null) : this(() => new[] {text.Invoke()}, spec) { }

        public Expander(Callbacks.BitContents text, Spec? spec = null)
        {
            IEnumerable<IRenderable> Contents() => text.Invoke().Append(Expanded ? UpIcon : DownIcon);

            _hook = new Hook();

            _button = new Button
            (
                Contents,
                new Button.Spec
                {
                    Style = () =>
                        spec?.Style?.Invoke() ??
                        (!Expanded ? Button.Style.Transparent : Button.Style.AccentTransparent),
                    IsSmall = null,
                    OnClick = (_, _) =>
                    {
                        Expanded = !Expanded;
                        _refresher?.Invoke();
                        _hook.Invoke();
                    },
                }
            );
        }

        public bool Expanded { get; private set; }

        public ReadOnlyHook Hook => _hook;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    $"I4E-Bit-Expander I4E-Bit-Expanded--{(Expanded ? "Expanded" : "Contracted")}");

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                builder.CloseElement();

                builder.AddContent(++seq, _button.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                builder.CloseElement();

                builder.CloseElement();
            },
            v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        public void Expand()
        {
            Expanded = true;
            _refresher?.Invoke();
            _hook.Invoke();
        }

        public void Contract()
        {
            Expanded = true;
            _refresher?.Invoke();
            _hook.Invoke();
        }

        public class Spec
        {
            public Button.StyleGetter? Style { get; init; }
        }
    }
}