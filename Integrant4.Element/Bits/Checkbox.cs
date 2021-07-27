using System;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    public partial class Checkbox : BitBase, IRefreshableBit
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : UnifiedSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<ushort>? Size { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.IsChecked?  IsChecked  { get; init; }

            public Callbacks.IsRequired? IsRequired { get; init; }

            public Callbacks.Classes? Classes         { get; init; }
            public Callbacks.Size?    Margin          { get; init; }
            public Callbacks.Size?    Padding         { get; init; }
            public Callbacks.Color?   ForegroundColor { get; init; }
            public Callbacks.Unit?    Height          { get; init; }
            public Callbacks.Unit?    Width           { get; init; }
            public Callbacks.Display? Display         { get; init; }
            public Callbacks.Data?    Data            { get; init; }
            public Callbacks.Tooltip? Tooltip         { get; init; }

            internal override SpecSet ToSpec() => new()
            {
                BaseClasses     = new ClassSet("I4E-Bit", "I4E-Bit-Checkbox"),
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                IsChecked       = IsChecked,
                IsRequired      = IsRequired,
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
        public Checkbox(Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _size     = spec?.Size                ?? (() => 25);
            IsChecked = spec?.IsChecked?.Invoke() ?? false;
        }
    }

    public partial class Checkbox
    {
        private WriteOnlyHook? _refresher;

        public bool IsChecked { get; private set; }

        public void Refresh() => _refresher?.Invoke();

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "onclick",
                EventCallback.Factory.Create<MouseEventArgs>(this, OnClick));

            string[] ac = { !IsChecked ? "I4E-Bit-Checkbox--Unchecked" : "I4E-Bit-Checkbox--Checked" };
            BitBuilder.ApplyOuterAttributes(this, builder, ref seq, ac);

            builder.OpenComponent<BootstrapIcon>(++seq);
            builder.AddAttribute(++seq, "ID", !IsChecked ? "square" : "check-square-fill");
            builder.AddAttribute(++seq, "Size", _size.Invoke());
            builder.CloseComponent();

            builder.CloseElement();
        }, v => _refresher = v);

        public event Action<Checkbox, bool>? OnToggle;

        private void OnClick(MouseEventArgs args)
        {
            if (OuterSpec?.IsDisabled?.Invoke() == true)
                return;

            IsChecked = !IsChecked;
            (_refresher ?? throw new ArgumentNullException()).Invoke();

            OnToggle?.Invoke(this, IsChecked);
        }

        public void Reset()
        {
            IsChecked = OuterSpec?.IsChecked?.Invoke() ?? false;
            (_refresher ?? throw new ArgumentNullException()).Invoke();
        }
    }

    public partial class Checkbox
    {
        private readonly Callbacks.Callback<ushort> _size;
    }
}