using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits.BitPresets
{
    public class HeaderSpacer : IBit
    {
        private static readonly RenderFragment Spacer;

        static HeaderSpacer()
        {
            Spacer = builder =>
            {
                builder.AddContent(0, new Space(() => 5).Renderer());
                builder.AddContent(1, new VerticalLine().Renderer());
                builder.AddContent(2, new Space(() => 5).Renderer());
            };
        }

        public RenderFragment Renderer() => Spacer;
    }
}