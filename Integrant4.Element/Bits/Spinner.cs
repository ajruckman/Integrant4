using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Spinner : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IUnifiedSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<string>? Text  { get; init; }
            public StyleGetter?                Style { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }
            public Callbacks.Size?       Margin     { get; init; }
            public Callbacks.REM?        FontSize   { get; init; }
            public Callbacks.FontWeight? FontWeight { get; init; }

            public SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Spinner)),
                IsVisible   = IsVisible,
            };
        }
    }

    public partial class Spinner
    {
        private readonly Callbacks.Callback<string>? _text;
        private readonly StyleGetter                 _style;

        private readonly Callbacks.Scale?      _scale;
        private readonly Callbacks.Size?       _margin;
        private readonly Callbacks.REM?        _fontSize;
        private readonly Callbacks.FontWeight? _fontWeight;

        public Spinner(Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _text  = spec?.Text;
            _style = spec?.Style ?? DefaultStyleGetter;

            _scale      = spec?.Scale;
            _margin     = spec?.Margin;
            _fontSize   = spec?.FontSize;
            _fontWeight = spec?.FontWeight;
        }
    }

    public partial class Spinner
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                BitBuilder.ApplyOuterAttributes(this, builder, ref seq, new[]
                {
                    "I4E-Bit-Spinner--" + _style.Invoke(),
                });

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Spinner-Inner");
                builder.AddAttribute(++seq, "style", new SpecSet
                {
                    Scaled = _scale != null,
                    Scale  = _scale,
                    Margin = _margin,
                }.StyleAttribute(null));
                builder.CloseElement();

                if (_text != null)
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Spinner-Text");
                    builder.AddAttribute(++seq, "style", new SpecSet
                    {
                        FontSize   = _fontSize,
                        FontWeight = _fontWeight,
                    }.StyleAttribute(null));
                    builder.AddContent(++seq, _text.Invoke());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }

    public partial class Spinner
    {
        public delegate Style StyleGetter();

        public enum Style
        {
            Block, Inline,
        }

        private static StyleGetter DefaultStyleGetter => () => Style.Block;
    }
}