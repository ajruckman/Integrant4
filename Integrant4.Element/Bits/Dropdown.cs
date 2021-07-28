using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Dropdown : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IUnifiedSpec
        {
            internal static readonly Spec Default = new();

            public PlacementExtensions.PlacementGetter? PlacementGetter { get; init; }

            public Callbacks.Classes? Classes { get; init; }
            public Callbacks.Size?    Margin  { get; init; }
            public Callbacks.Size?    Padding { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-Dropdown"),
                Classes     = Classes,
                Margin      = Margin,
                Padding     = Padding,
            };
        }
    }

    public partial class Dropdown
    {
        private readonly ContentRef _headContents;
        private readonly ContentRef _childContents;

        public Dropdown
        (
            ContentRef headContents,
            ContentRef childContents,
            Spec?      spec = null
        ) : base(spec ?? Spec.Default)
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
            private ElementReference? _contentRef;
            private ElementService?   _elementService;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyOuterAttributes(Dropdown, builder, ref seq);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id",    Dropdown.ID + ".Head");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Head");
                builder.AddElementReferenceCapture(++seq, r => _headRef = r);

                foreach (IRenderable renderable in Dropdown._headContents.GetAll())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id",    Dropdown.ID + ".Contents");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Contents");
                builder.AddAttribute(++seq, "data-popper-placement",
                    (Dropdown._placementGetter?.Invoke() ?? TooltipPlacement.Bottom).Map());
                builder.AddElementReferenceCapture(++seq, r => _contentRef = r);

                foreach (IRenderable renderable in Dropdown._childContents.GetAll())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Dropdown-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                // if (_toggleRef != null && _contentRef != null)
                // BaseSpec.ElementService!.AddJob(async v =>
                // await v.InvokeVoidAsync("I4.Element.InitDropdown", _toggleRef!, _contentRef!));
            }

            protected override void OnAfterRender(bool firstRender)
            {
                if (firstRender)
                    _elementService!.AddJob((j, t) =>
                        Interop.CallVoid(j, t, "I4.Element.InitDropdown", _headRef!.Value, _contentRef!.Value));
            }
        }
    }

    public partial class Dropdown
    {
        private readonly PlacementExtensions.PlacementGetter? _placementGetter;
    }
}