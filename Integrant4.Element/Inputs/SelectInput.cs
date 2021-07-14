using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class SelectInput<TValue> : StandardInput<TValue?>
    {
        public interface IOption
        {
            TValue? Value    { get; }
            Content Content  { get; }
            bool    Selected { get; }
            bool    Disabled { get; }
        }

        public class Option : IOption
        {
            public Option
            (
                TValue? value,
                Content content,
                bool    selected = false,
                bool    disabled = false
            )
            {
                Value    = value;
                Content  = content;
                Selected = selected;
                Disabled = disabled;
            }

            public TValue? Value    { get; }
            public Content Content  { get; }
            public bool    Selected { get; }
            public bool    Disabled { get; }
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Unit?       Height          { get; init; }
            public Callbacks.Unit?       HeightMax       { get; init; }
            public Callbacks.Unit?       Width           { get; init; }
            public Callbacks.Unit?       WidthMax        { get; init; }
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Scaled = true,

                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                IsRequired      = IsRequired,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                HeightMax       = HeightMax,
                Width           = Width,
                WidthMax        = WidthMax,
                Scale           = Scale,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class SelectInput<TValue>
    {
        public delegate IReadOnlyList<IOption> OptionGetter();

        public delegate bool OptionEqualityComparer(TValue? left, TValue? right);

        private readonly OptionGetter           _optionGetter;
        private readonly OptionEqualityComparer _optionEqualityComparer;
        private readonly object                 _optionCacheLock = new();

        private IReadOnlyList<IOption>? _optionCache;

        public SelectInput
        (
            IJSRuntime              jsRuntime,
            TValue?                 value,
            OptionGetter            optionGetter,
            OptionEqualityComparer? optionEqualityComparer = null,
            Spec?                   spec                   = null
        ) : base(jsRuntime, spec?.ToBaseSpec(), new ClassSet("I4E-Input", "I4E-Input-SelectInput"))
        {
            _optionGetter = optionGetter;

            if (optionEqualityComparer != null)
                _optionEqualityComparer = optionEqualityComparer;
            else if (typeof(IEquatable<TValue>).IsAssignableFrom(typeof(TValue)))
                _optionEqualityComparer = (left, right) =>
                    left == null && right == null || left?.Equals(right) == true;
            else
                throw new ArgumentException(
                    "TValue does not implement IEquatable<TValue> and no OptionEqualityComparer was passed.");

            Value = Nullify(value);
        }

        public void InvalidateCache()
        {
            lock (_optionCacheLock)
            {
                _optionCache = null;
            }
        }

        private IReadOnlyList<IOption> Options()
        {
            lock (_optionCacheLock)
            {
                return _optionCache ??= _optionGetter.Invoke();
            }
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Input I4E-Input-Select");

            builder.OpenElement(++seq, "select");
            builder.AddAttribute(++seq, "oninput",  EventCallback.Factory.Create(this, Change));
            builder.AddAttribute(++seq, "disabled", BaseSpec.IsDisabled?.Invoke());
            builder.AddAttribute(++seq, "required", BaseSpec.IsRequired?.Invoke());

            lock (_optionCacheLock)
            {
                IReadOnlyList<IOption> options = Options();

                for (var i = 0; i < options.Count; i++)
                {
                    IOption option = options[i];

                    builder.OpenElement(++seq, "option");
                    builder.AddAttribute(++seq, "value", i);

                    ++seq;
                    if (_optionEqualityComparer.Invoke(Value, option.Value))
                        builder.AddAttribute(seq, "selected", true);

                    ++seq;
                    if (option.Selected)
                        builder.AddAttribute(seq, "selected", true);

                    ++seq;
                    if (option.Disabled)
                        builder.AddAttribute(seq, "disabled", true);

                    builder.AddContent(++seq, option.Content.Fragment);
                    builder.CloseElement();
                }
            }

            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        protected override string Serialize(TValue? v) => v?.ToString() ?? "";

        protected override TValue? Deserialize(string? v)
        {
            if (string.IsNullOrEmpty(v)) return default;

            int index = int.Parse(v);

            return Options()[index].Value;
        }

        protected sealed override TValue? Nullify(TValue? v) => v;

        private void Change(ChangeEventArgs args)
        {
            lock (_optionCacheLock)
            {
                InvokeOnChange(Deserialize(args.Value?.ToString()));
            }
        }
    }
}