using Integrant4.API;
using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

// ReSharper disable RedundantArgumentDefaultValue

namespace Integrant4.Element.Constructs
{
    public class PanelExpander : ComponentBase
    {
        private static readonly BootstrapIcon DownIcon = new("caret-down-fill", 16);
        private static readonly BootstrapIcon UpIcon   = new("caret-up-fill", 16);

        [Parameter] public Callbacks.BitContents HeaderElements  { get; set; } = null!;
        [Parameter] public RenderFragment        ChildContent    { get; set; } = null!;
        [Parameter] public Callbacks.BitContent? ExpandContent   { get; set; }
        [Parameter] public Callbacks.BitContent? ContractContent { get; set; }
        [Parameter] public bool                  Expanded        { get; set; }

        private Header _header = null!;

        protected override void OnInitialized()
        {
            ExpandContent   ??= () => "Click to show".AsContent();
            ContractContent ??= () => "Click to hide".AsContent();

            Button button = new
            (
                () =>
                {
                    var right = new IRenderable[2];
                    if (!Expanded)
                    {
                        right[0] = ExpandContent.Invoke();
                        right[1] = DownIcon;
                    }
                    else
                    {
                        right[0] = ContractContent.Invoke();
                        right[1] = UpIcon;
                    }


                    return new IRenderable[]
                    {
                        new RenderableArray(HeaderElements.Invoke()),
                        new RenderableArray(right),
                    };
                },
                new Button.Spec
                {
                    Style   = () => !Expanded ? Button.Style.Transparent : Button.Style.AccentTransparent,
                    IsSmall = null,
                    OnClick = (_, _) =>
                    {
                        Expanded = !Expanded;
                        InvokeAsync(StateHasChanged);
                    },
                    Classes     = () => new ClassSet("I4E-Construct-PanelExpander-PanelButton"),
                    FlexJustify = () => FlexJustify.SpaceBetween,
                }
            );

            _header = new Header(() => new IRenderable[]
            {
                button,
            }, Header.Style.Secondary);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-PanelExpander");

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                $"I4E-Layout-Panel I4E-Layout-Panel--Expandable I4E-Layout-Panel--{(Expanded ? "Expanded" : "Contracted")}");

            builder.AddContent(++seq, _header.Renderer());

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Layout-Panel-Inner");
            builder.AddAttribute(++seq, "hidden", !Expanded);
            builder.AddContent(++seq, ChildContent);
            builder.CloseElement();

            builder.CloseElement();

            builder.CloseElement();
        }
    }
}