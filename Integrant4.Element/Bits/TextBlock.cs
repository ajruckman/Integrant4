using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class TextBlock : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Unit?       Height          { get; init; }
            public Callbacks.Unit?       HeightMax       { get; init; }
            public Callbacks.Unit?       Width           { get; init; }
            public Callbacks.Unit?       WidthMax        { get; init; }
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.TextAlign?  TextAlign       { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
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
                TextAlign       = TextAlign,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class TextBlock
    {
        private readonly DynamicContents _contents;

        public TextBlock(DynamicContents contents, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(TextBlock)))
        {
            _contents = contents;
        }

        public TextBlock(DynamicContent content, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(TextBlock)))
        {
            _contents = content.AsDynamicContents();
        }

        public TextBlock(IRenderable content, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(TextBlock)))
        {
            _contents = () => new[] { content };
        }
    }

    public partial class TextBlock
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyAttributes(this, builder, ref seq, null, null);

                foreach (IRenderable renderable in _contents.Invoke())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-TextBlock-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                BitBuilder.ScheduleElementJobs(this, builder, ref seq);
            }

            return Fragment;
        }
    }

    public partial class TextBlock
    {
        // public static readonly Spec SecondaryHeaderStyle;
        //
        // static TextBlock()
        // {
        //     SecondaryHeaderStyle = new Spec
        //     {
        //         FontWeight = () => FontWeight.SemiBold,
        //         FontSize   = () => 1.2,
        //     };
        // }
    }
}