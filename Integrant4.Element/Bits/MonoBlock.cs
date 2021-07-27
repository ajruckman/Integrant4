using System.Diagnostics.CodeAnalysis;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public partial class MonoBlock : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : UnifiedSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal override SpecSet ToSpec() => new()
            {
                BaseClasses = new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(MonoBlock)),
                IsVisible       = IsVisible,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class MonoBlock
    {
        private readonly ContentRef _content;

        public MonoBlock(ContentRef content, Spec? spec = null) : base(spec ?? Spec.Default)
        {
            _content = content;
        }
    }

    public partial class MonoBlock
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");

                BitBuilder.ApplyOuterAttributes(this, builder, ref seq);

                foreach (IRenderable renderable in _content.GetAll())
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}