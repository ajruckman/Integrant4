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
        public class Spec
        {
            public Callbacks.Callback<string>? Text  { get; init; }
            public StyleGetter?                Style { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }
            public Callbacks.REM?        FontSize   { get; init; }
            public Callbacks.FontWeight? FontWeight { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
            };
        }
    }

    public partial class Spinner
    {
        private readonly Callbacks.Callback<string>? _text;
        private readonly StyleGetter                 _style;

        private readonly Callbacks.Scale?      _scale;
        private readonly Callbacks.REM?        _fontSize;
        private readonly Callbacks.FontWeight? _fontWeight;

        public Spinner(Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Spinner)))
        {
            _text  = spec?.Text;
            _style = spec?.Style ?? DefaultStyleGetter;

            _scale      = spec?.Scale;
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
                BitBuilder.ApplyAttributes(this, builder, ref seq, new[]
                {
                    "I4E-Bit-Spinner--" + _style.Invoke(),
                }, null);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Spinner-Inner");
                builder.AddAttribute(++seq, "style", ElementBuilder.StyleAttribute(new BaseSpec
                {
                    Scaled = _scale != null,
                    Scale  = _scale,
                }, null));
                builder.CloseElement();

                if (_text != null)
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Spinner-Text");
                    builder.AddAttribute(++seq, "style", ElementBuilder.StyleAttribute(new BaseSpec
                    {
                        FontSize   = _fontSize,
                        FontWeight = _fontWeight,
                    }, null));
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
            Block,
            Inline,
        }

        private static StyleGetter DefaultStyleGetter => () => Style.Block;
    }
}