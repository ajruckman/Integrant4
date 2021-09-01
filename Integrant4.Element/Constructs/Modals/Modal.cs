using System;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Modals
{
    public class Modal
    {
        public bool Visible { get; private set; }

        public void Show()
        {
            if (Visible) return;
            Visible = true;
            OnChange?.Invoke(Visible);
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
            OnChange?.Invoke(Visible);
        }

        public event Action<bool>? OnChange;
    }

    public class ModalContent : ComponentBase
    {
        private WriteOnlyHook?    _refresher;
        private ElementReference? _elementRef;
        private StyleSet?         _style = null;

        [Parameter] public Modal          Modal        { get; set; } = null!;
        [Parameter] public RenderFragment ChildContent { get; set; } = null!;

        [Parameter] public Unit? MinWidth  { get; set; }
        [Parameter] public Unit? MinHeight { get; set; }

        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnParametersSet()
        {
            if (Modal == null)
            {
                throw new ArgumentException("Null Modal passed to ModalContent component.", nameof(Modal));
            }

            Modal.OnChange += _ => InvokeAsync(StateHasChanged);

            _style = new StyleSet
            {
                { "min-width", MinWidth?.Serialize() },
                { "min-height", MinHeight?.Serialize() },
                { "display", "none" },
            };
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = -1;

            builder.OpenElement(++seq, "section");
            builder.AddAttribute(++seq, "hidden", !Modal.Visible);

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-ModalBackground");
            builder.CloseElement();

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-ModalContent");
            builder.AddAttribute(++seq, "style", _style?.ToString());
            builder.AddElementReferenceCapture(++seq, r => _elementRef = r);
            builder.AddContent(++seq, ChildContent);
            builder.CloseElement();

            builder.CloseElement();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_elementRef == null) return;
            await ElementService.JSInvokeVoidAsync
            (
                "I4.Element.CenterModal", _elementRef!.Value
            );
        }
    }
}