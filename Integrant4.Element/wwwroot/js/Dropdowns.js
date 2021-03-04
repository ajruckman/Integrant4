window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.DropdownShowEvents = ['mouseenter', 'focus'];
window.I4.Element.DropdownHideEvents = ['mouseleave', 'blur'];

window.I4.Element.InitDropdown = window.I4.Element.InitDropdown || function (toggle, contents) {
    if (!toggle.hasOwnProperty('I4EBitDropdown')) {
        toggle.I4EBitDropdown = Popper.createPopper(toggle, contents, {
            placement: contents.getAttribute('data-popper-placement'),
        });

        window.I4.Element.DropdownShowEvents.forEach(event => {
            toggle.addEventListener(event, () => {
                contents.setAttribute('data-show', '');

                toggle.I4EBitDropdown.setOptions({
                    modifiers: [{name: 'eventListeners', enabled: true}],
                });

                toggle.I4EBitDropdown.update();
            });
        });

        window.I4.Element.DropdownHideEvents.forEach(event => {
            toggle.parentElement.addEventListener(event, () => contents.removeAttribute('data-show'));

            toggle.I4EBitDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: false}],
            });
        });
    }
}
