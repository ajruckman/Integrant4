using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.API;
using Integrant4.Colorant.Themes.Default;
using Integrant4.Element.Constructs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public class ExpanderPanel : ComponentBase
    {
        private static readonly BootstrapIcon DownIcon  = new("caret-down-fill", 16, Constants.Text_2);
        private static readonly BootstrapIcon UpIcon    = new("caret-up-fill", 16, Constants.Text_2);
        private                 Expander      _expander = null!;

        [Parameter] public IRenderable[]         HeaderElements  { get; set; } = null!;
        [Parameter] public RenderFragment        ChildContent    { get; set; } = null!;
        [Parameter] public Callbacks.BitContent? ExpandContent   { get; set; }
        [Parameter] public Callbacks.BitContent? ContractContent { get; set; }

        protected override void OnInitialized()
        {
            ExpandContent   ??= () => "Show".AsContent();
            ContractContent ??= () => "Hide".AsContent();

            _expander = new Expander
            (
                ExpandContent,
                ContractContent,
                HeaderElements,
                ChildContent
            );
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _expander.Renderer());
        }
    }

    public partial class Expander : IRefreshableBit
    {
        private static readonly BootstrapIcon DownIcon = new("caret-down-fill", 16);
        private static readonly BootstrapIcon UpIcon   = new("caret-up-fill", 16);

        private readonly Hook  _hook;
        private readonly Style _style;

        private readonly Button? _button;

        private readonly Header?         _header;
        private readonly RenderFragment? _childContent;

        private WriteOnlyHook? _refresher;

        public Expander
        (
            Callbacks.BitContent expandContent,
            Callbacks.BitContent contractContent,
            Spec?                spec = null
        )
        {
            _hook   = new Hook();
            _style  = Style.Button;
            _button = BuildButton(expandContent, contractContent, spec);
        }

        public Expander
        (
            Callbacks.BitContent expandContent,
            Callbacks.BitContent contractContent,
            IRenderable[]        headerElements,
            RenderFragment       childContent,
            Spec?                spec = null
        )
        {
            _hook  = new Hook();
            _style = Style.Panel;

            _header = new Header(() => new IRenderable[]
            {
                new RenderableArray(headerElements),
                new Space(new Space.Spec
                {
                    Width = () => 15,
                }),
                BuildButton(expandContent, contractContent, spec),
            }, Header.Style.Secondary, new Header.Spec
            {
                Clickable = true,
                Padding   = () => new Size(0, 2, 0, 10),
            });

            _childContent = childContent;
        }

        public bool Expanded { get; private set; }

        public ReadOnlyHook Hook => _hook;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                int seq = -1;

                if (_style == Style.Button)
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class",
                        $"I4E-Bit-Expander I4E-Bit-Expanded--{(Expanded ? "Expanded" : "Contracted")}");

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                    builder.CloseElement();

                    builder.AddContent(++seq, _button!.Renderer());

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                    builder.CloseElement();

                    builder.CloseElement();
                }
                else if (_style == Style.Panel)
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class",
                        $"I4E-Layout-Panel I4E-Layout-Panel--{(Expanded ? "Expanded" : "Contracted")}");

                    builder.AddContent(++seq, _header!.Renderer());

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class",  "I4E-Layout-Panel-Inner");
                    builder.AddAttribute(++seq, "hidden", !Expanded);
                    builder.AddContent(++seq, _childContent!);
                    builder.CloseElement();

                    builder.CloseElement();
                }
            },
            v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        private Button BuildButton
        (
            Callbacks.BitContent expandContent,
            Callbacks.BitContent contractContent,
            Spec?                spec = null
        )
        {
            IEnumerable<IRenderable> Contents() => Expanded
                ? new[] { contractContent.Invoke(), UpIcon }
                : new[] { expandContent.Invoke(), DownIcon };

            return new Button
            (
                Contents,
                new Button.Spec
                {
                    Style = () =>
                        spec?.ButtonStyle?.Invoke() ??
                        (!Expanded ? Button.Style.Transparent : Button.Style.AccentTransparent),
                    IsSmall = null,
                    OnClick = (_, _) =>
                    {
                        Expanded = !Expanded;
                        _refresher?.Invoke();
                        _hook.Invoke();
                    },
                    Classes     = () => new ClassSet("I4E-Bit-Expander-PanelButton"),
                    FlexJustify = () => FlexJustify.End,
                }
            );
        }

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
    }

    public partial class Expander
    {
        public enum Style
        {
            Button, Panel,
        }

        public static readonly IRenderable DefaultExpandableHeaderNotice;

        static Expander()
        {
            DefaultExpandableHeaderNotice = new RenderableArray(new IRenderable[]
            {
                new Filler(),
                new TextBlock("Click to expand".AsContent(), new TextBlock.Spec
                {
                    Padding         = () => new Size(0, 3),
                    ForegroundColor = () => Constants.Text_2,
                }),
            });
        }

        public class Spec
        {
            public Button.StyleGetter? ButtonStyle { get; init; }
        }
    }
}