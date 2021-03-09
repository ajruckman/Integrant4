using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Superset.Web.State;

namespace Integrant4.Element.Inputs
{
    public interface IOption<out TValue>
    {
        TValue? Value            { get; }
        Content OptionContent    { get; }
        Content SelectionContent { get; }
        bool    Selected         { get; }
        bool    Disabled         { get; }
    }

    public class Option<TValue> : IOption<TValue>
    {
        public Option
        (
            TValue?  value,
            Content  optionText,
            Content? selectionContent = null,
            bool     selected         = false,
            bool     disabled         = false)
        {
            Value            = value;
            OptionContent    = optionText;
            SelectionContent = selectionContent ?? optionText;
            Selected         = selected;
            Disabled         = disabled;
        }

        public TValue? Value            { get; }
        public Content OptionContent    { get; }
        public Content SelectionContent { get; }
        public bool    Selected         { get; }
        public bool    Disabled         { get; }
    }

    public partial class SelectInput<TValue> : StandardInput<TValue?>, ICachingInput
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Spec(OptionGetter optionGetter)
            {
                OptionGetter = optionGetter;
            }

            public OptionGetter            OptionGetter           { get; }
            public OptionEqualityComparer? OptionEqualityComparer { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Pixels?     Height          { get; init; }
            public Callbacks.Pixels?     HeightMax       { get; init; }
            public Callbacks.Pixels?     Width           { get; init; }
            public Callbacks.Pixels?     WidthMax        { get; init; }
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
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
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class SelectInput<TValue>
    {
        public delegate List<IOption<TValue>> OptionGetter();

        public delegate bool OptionEqualityComparer(TValue? left, TValue? right);

        private readonly OptionGetter           _optionGetter;
        private readonly OptionEqualityComparer _optionEqualityComparer;
        private readonly object                 _optionCacheLock = new();
        private readonly UpdateTrigger          _signaler        = new();

        private List<IOption<TValue>>? _optionCache;

        public SelectInput
        (
            IJSRuntime jsRuntime,
            TValue?    value,
            Spec       spec
        ) : base(jsRuntime, spec.ToBaseSpec(), new ClassSet("I4E-Input", "I4E-Input-SelectInput"))
        {
            _optionGetter = spec.OptionGetter;

            if (spec.OptionEqualityComparer != null)
                _optionEqualityComparer = spec.OptionEqualityComparer;
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
                _signaler.Trigger();
            }
        }

        private List<IOption<TValue>> Options()
        {
            lock (_optionCacheLock)
            {
                return _optionCache ??= _optionGetter.Invoke();
            }
        }

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenComponent<TriggerWrapper>(++seq);
                builder.AddAttribute(++seq, "Trigger", _signaler);
                builder.AddAttribute(++seq, "ChildContent", (RenderFragment) (builder2 =>
                {
                    builder2.OpenElement(++seq, "div");
                    builder2.AddAttribute(++seq, "class", "I4E-Input I4E-Input-Select");

                    builder2.OpenElement(++seq, "select");
                    builder2.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                    lock (_optionCacheLock)
                    {
                        int seqI = -1;
                        for (var i = 0; i < Options().Count; i++)
                        {
                            IOption<TValue> option = Options()[i];

                            builder2.OpenElement(++seqI, "option");
                            builder2.AddAttribute(++seqI, "value", i);

                            ++seqI;
                            if (_optionEqualityComparer.Invoke(Value, option.Value))
                                builder2.AddAttribute(seqI, "selected", true);

                            ++seqI;
                            if (option.Disabled)
                                builder2.AddAttribute(seqI, "disabled", true);

                            builder2.AddContent(++seqI, option.OptionContent.Fragment);
                            builder2.CloseElement();
                        }
                    }

                    builder2.CloseElement();

                    builder2.CloseElement();
                }));
                builder.CloseComponent();

                InputBuilder.ScheduleElementJobs(this, builder, ref seq);
            }

            return Fragment;
        }

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