using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class Chip : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IDualSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.IsVisible? IsVisible { get; init; }

            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.HREF?       HREF            { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Unit?       Height          { get; init; }
            public Callbacks.Unit?       Width           { get; init; }
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            public SpecSet ToOuterSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Chip),
                    HREF == null
                        ? "I4E-Bit-" + nameof(Chip) + "--Static"
                        : "I4E-Bit-" + nameof(Chip) + "--Link"),
                Scaled          = true,
                IsVisible       = IsVisible,
                Classes         = Classes,
                HREF            = HREF,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                Width           = Width,
                Scale           = Scale,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };

            public SpecSet ToInnerSpec() => new()
            {
                FontWeight = FontWeight,
            };
        }
    }

    public partial class Chip
    {
        private readonly ContentRef _content;

        public Chip(ContentRef content, Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _content = content;
        }
    }

    public partial class Chip
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                IRenderable[] contents = _content.GetAll().ToArray();

                List<string> ac = new();

                if (contents.First() is IIcon) ac.Add("I4E-Bit-Chip--IconLeft");
                if (contents.Last() is IIcon) ac.Add("I4E-Bit-Chip--IconRight");

                //

                int seq = -1;

                if (OuterSpec?.HREF == null)
                {
                    builder.OpenElement(++seq, "div");
                }
                else
                {
                    builder.OpenElement(++seq, "a");
                    builder.AddAttribute(++seq, "href", OuterSpec?.HREF.Invoke());
                }

                BitBuilder.ApplyOuterAttributes(this, builder, ref seq, ac);

                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Chip-Contents");

                BitBuilder.ApplyInnerAttributes(this, builder, ref seq);

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Chip-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();

                BitBuilder.ScheduleElementJobs(this, builder, ref seq);
            }

            return Fragment;
        }
    }
}