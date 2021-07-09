using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits.BitPresets
{
    public class DropdownLinkButton : IBit
    {
        private readonly Button _button;

        public DropdownLinkButton(DynamicContents contents, Callbacks.HREF href)
        {
            _button = new Button(contents, new Button.Spec { HREF = href, Style = () => Button.Style.Transparent });
        }

        public RenderFragment Renderer() => _button.Renderer();
    }
}