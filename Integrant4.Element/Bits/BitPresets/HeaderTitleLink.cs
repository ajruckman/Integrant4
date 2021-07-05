using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits.BitPresets
{
    public class HeaderTitleLink : IBit
    {
        private readonly HeaderLink _headerLink;

        public HeaderTitleLink(DynamicContents contents, Callbacks.HREF? href = null)
        {
            _headerLink = new HeaderLink(contents, href ?? (() => "/"), new HeaderLink.Spec
            {
                IsTitle = Always.True,
            });
        }

        public RenderFragment Renderer() => _headerLink.Renderer();
    }
}