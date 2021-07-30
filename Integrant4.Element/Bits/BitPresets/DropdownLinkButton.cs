using System;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits.BitPresets
{
    public class DropdownLinkButton : IBit
    {
        private readonly Button _button;

        public DropdownLinkButton(ContentRef content, Callbacks.HREF href, Action<Button, ClickArgs>? onClick = null)
        {
            _button = new Button(content, new Button.Spec
            {
                HREF    = href,
                Style   = () => Button.Style.Transparent,
                OnClick = onClick,
            });
        }

        public RenderFragment Renderer() => _button.Renderer();
    }
}