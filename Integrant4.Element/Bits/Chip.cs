using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Chip : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.IsVisible? IsVisible { get; init; }

            // public Callbacks.BitID?        ID              { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.HREF?       HREF            { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Pixels?     Height          { get; init; }
            public Callbacks.Pixels?     Width           { get; init; }
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                IsVisible = IsVisible,
                // ID              = ID,
                Classes         = Classes,
                HREF            = HREF,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                Width           = Width,
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class Chip
    {
        private readonly Callbacks.BitContents _contents;

        public Chip(Callbacks.BitContent content, Spec? spec = null)
            : this(content.AsContents(), spec)
        {
        }

        public Chip(Callbacks.BitContents contents, Spec? spec = null)
            : base(spec?.ToBaseSpec(),
                new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Chip),
                    spec?.HREF == null
                        ? "I4E-Bit-" + nameof(Chip) + "--Static"
                        : "I4E-Bit-" + nameof(Chip) + "--Link"))
        {
            _contents = contents;
        }
    }

    public partial class Chip
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                if (BaseSpec.HREF == null)
                {
                    builder.OpenElement(++seq, "div");
                }
                else
                {
                    builder.OpenElement(++seq, "a");
                    builder.AddAttribute(++seq, "href", BaseSpec.HREF.Invoke());
                }

                BitBuilder.ApplyAttributes(this, builder, ref seq, null, null);

                foreach (IRenderable renderable in _contents.Invoke())
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }

                builder.CloseElement();

                BitBuilder.ScheduleElementJobs(this, builder, ref seq);
            }

            return Fragment;
        }
    }
}