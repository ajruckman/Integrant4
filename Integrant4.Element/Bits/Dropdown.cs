using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Bits
{
    public partial class Dropdown : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public PlacementExtensions.PlacementGetter? PlacementGetter { get; init; }

            public Callbacks.Classes? Classes { get; init; }
            public Callbacks.Size?    Margin  { get; init; }
            public Callbacks.Size?    Padding { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Classes = Classes,
                Margin  = Margin,
                Padding = Padding,
            };
        }
    }

    public partial class Dropdown
    {
        private readonly Callbacks.BitContents _headContents;
        private readonly Callbacks.BitContents _childContents;

        public Dropdown
        (
            Callbacks.BitContents headContents,
            Callbacks.BitContents childContents,
            Spec?                 spec = null
        ) : base(spec?.ToBaseSpec(),
            new ClassSet("I4E-Bit", "I4E-Bit-Dropdown"))
        {
            _headContents    = headContents;
            _childContents   = childContents;
            _placementGetter = spec?.PlacementGetter;
        }
    }

    public partial class Dropdown
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Component>(0);
                builder.AddAttribute(1, "Dropdown", this);
                builder.CloseComponent();
            }

            return Fragment;
        }
    }

    public partial class Dropdown
    {
        private class Component : ComponentBase
        {
            [Parameter] public Dropdown Dropdown { get; set; } = null!;

            private ElementReference? _headRef;
            private ElementReference? _childrenRef;
            private ElementService?   _elementService;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(Dropdown, builder, ref seq, null, null);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id", Dropdown.ID + ".Head");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Head");
                builder.AddElementReferenceCapture(++seq, r => _headRef = r);

                foreach (IRenderable renderable in Dropdown._headContents.Invoke())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id", Dropdown.ID + ".Contents");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Children");
                builder.AddAttribute(++seq, "data-popper-placement",
                    (Dropdown._placementGetter?.Invoke() ?? Placement.Bottom).Map());
                builder.AddElementReferenceCapture(++seq, r => _childrenRef = r);

                foreach (IRenderable renderable in Dropdown._childContents.Invoke())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                // if (_toggleRef != null && _contentsRef != null)
                // BaseSpec.ElementService!.AddJob(async v =>
                // await v.InvokeVoidAsync("I4.Element.InitDropdown", _toggleRef!, _contentsRef!));
            }

            protected override void OnAfterRender(bool firstRender)
            {
                if (firstRender)
                    _elementService!.AddJob((j, t) =>
                        Interop.InitDropdown(j, t, _headRef!.Value, _childrenRef!.Value));
            }
        }
    }

    public partial class Dropdown
    {
        private readonly PlacementExtensions.PlacementGetter? _placementGetter;
    }
}