using System;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    public partial class Checkbox : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.Callback<ushort>? Size { get; init; }

            public Callbacks.BitIsVisible?  IsVisible       { get; init; }
            public Callbacks.BitIsDisabled? IsDisabled      { get; init; }
            public Callbacks.BitIsChecked?  IsChecked       { get; init; }
            public Callbacks.BitIsRequired? IsRequired      { get; init; }
            // public Callbacks.BitID?         ID              { get; init; }
            public Callbacks.BitClasses?    Classes         { get; init; }
            public Callbacks.BitSize?       Margin          { get; init; }
            public Callbacks.BitSize?       Padding         { get; init; }
            public Callbacks.BitColor?      ForegroundColor { get; init; }
            public Callbacks.BitPixels?     Height          { get; init; }
            public Callbacks.BitPixels?     Width           { get; init; }
            public Callbacks.BitDisplay?    Display         { get; init; }
            public Callbacks.BitData?       Data            { get; init; }
            public Callbacks.BitTooltip?    Tooltip         { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                IsChecked       = IsChecked,
                IsRequired      = IsRequired,
                // ID              = ID,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                Width           = Width,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class Checkbox
    {
        public Checkbox(Spec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E.Bit", "I4E.Bit.Checkbox"))
        {
            _size     = spec?.Size                ?? (() => 25);
            IsChecked = spec?.IsChecked?.Invoke() ?? false;
        }
    }

    public partial class Checkbox
    {
        public bool IsChecked { get; private set; }

        private Action? _stateHasChanged;

        public event Action<bool>? OnToggle;

        private void OnClick(MouseEventArgs args)
        {
            if (BaseSpec.IsDisabled?.Invoke() == true)
                return;

            IsChecked = !IsChecked;
            (_stateHasChanged ?? throw new ArgumentNullException()).Invoke();

            OnToggle?.Invoke(IsChecked);
        }

        public void Reset()
        {
            IsChecked = BaseSpec.IsChecked?.Invoke() ?? false;
            (_stateHasChanged ?? throw new ArgumentNullException()).Invoke();
        }

        private void SetStateHasChanged(Action stateHasChanged)
        {
            _stateHasChanged = stateHasChanged;
        }

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenComponent<Component>(++seq);
                builder.AddAttribute(++seq, "Checkbox", this);
                builder.CloseComponent();
            }

            return Fragment;
        }
    }

    public partial class Checkbox
    {
        private readonly Callbacks.Callback<ushort> _size;
    }

    public partial class Checkbox
    {
        private class Component : ComponentBase
        {
            [Parameter] public Checkbox Checkbox { get; set; } = null!;

            protected override void OnInitialized()
            {
                Checkbox.SetStateHasChanged(() => InvokeAsync(StateHasChanged));
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "onclick",
                    EventCallback.Factory.Create<MouseEventArgs>(this, Checkbox.OnClick));

                string[] ac = {!Checkbox.IsChecked ? "I4E.Bit.Checkbox--Unchecked" : "I4E.Bit.Checkbox--Checked"};
                BitBuilder.ApplyAttributes(Checkbox, builder, ref seq, ac, null);

                builder.OpenComponent<BootstrapIcon>(++seq);
                builder.AddAttribute(++seq, "Name", !Checkbox.IsChecked ? "square" : "check-square-fill");
                builder.AddAttribute(++seq, "Size", Checkbox._size.Invoke());
                builder.CloseComponent();

                builder.CloseElement();
            }
        }
    }
}