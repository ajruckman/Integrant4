using System;
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
            public Spec(ElementService elementService)
            {
                ElementService = elementService;
            }

            public ElementService ElementService { get; init; }

            public PlacementGetter? PlacementGetter { get; init; }

            public Callbacks.BitClasses? Classes { get; init; }
            public Callbacks.BitSize?    Margin  { get; init; }
            public Callbacks.BitSize?    Padding { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                ElementService = ElementService,
                Classes        = Classes,
                Margin         = Margin,
                Padding        = Padding,
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
            Spec                  spec
        ) : base(spec.ToBitSpec(),
            new ClassSet("I4E.Bit", "I4E.Bit.Dropdown"))
        {
            _headContents    = headContents;
            _childContents   = childContents;
            _placementGetter = spec.PlacementGetter ?? DefaultPlacementGetter;
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

            private ElementReference? _toggleRef;
            private ElementReference? _contentsRef;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(Dropdown, builder, ref seq, null, null);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id", Dropdown.ID + ".Head");
                builder.AddAttribute(++seq, "class", "I4E.Bit.Dropdown.Head");
                builder.AddElementReferenceCapture(++seq, r => _toggleRef = r);

                foreach (IRenderable renderable in Dropdown._headContents.Invoke())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E.Bit.Dropdown.Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id", Dropdown.ID + ".Contents");
                builder.AddAttribute(++seq, "class", "I4E.Bit.Dropdown.Children");
                builder.AddAttribute(++seq, "data-popper-placement", Map(Dropdown._placementGetter.Invoke()));
                builder.AddElementReferenceCapture(++seq, r => _contentsRef = r);

                foreach (IRenderable renderable in Dropdown._childContents.Invoke())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E.Bit.Dropdown.Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();

                // if (_toggleRef != null && _contentsRef != null)
                // BaseSpec.ElementService!.AddJob(async v =>
                // await v.InvokeVoidAsync("I4.Element.InitDropdown", _toggleRef!, _contentsRef!));
            }

            protected override void OnAfterRender(bool firstRender)
            {
                if (firstRender)
                    Dropdown.BaseSpec.ElementService!.AddJob(async v =>
                        await v.InvokeVoidAsync("I4.Element.InitDropdown", _toggleRef!, _contentsRef!));
            }
        }
    }

    public partial class Dropdown
    {
        public delegate Placement PlacementGetter();

        public enum Placement
        {
            Auto,
            TopStart,
            Top,
            TopEnd,
            RightStart,
            Right,
            RightEnd,
            BottomEnd,
            Bottom,
            BottomStart,
            LeftEnd,
            Left,
            LeftStart,
        }

        private static readonly PlacementGetter DefaultPlacementGetter = () => Placement.Bottom;

        private readonly PlacementGetter _placementGetter;

        private static string Map(Placement placement) => placement switch
        {
            Placement.Auto        => "auto",
            Placement.TopStart    => "top-start",
            Placement.Top         => "top",
            Placement.TopEnd      => "top-end",
            Placement.RightStart  => "right-start",
            Placement.Right       => "right",
            Placement.RightEnd    => "right-end",
            Placement.BottomEnd   => "bottom-end",
            Placement.Bottom      => "bottom",
            Placement.BottomStart => "bottom-start",
            Placement.LeftEnd     => "left-end",
            Placement.Left        => "left",
            Placement.LeftStart   => "left-start",
            _                     => throw new ArgumentOutOfRangeException(nameof(placement), placement, null),
        };
    }
}