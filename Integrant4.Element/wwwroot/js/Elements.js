window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

//

window.I4.Element.InitTooltip = window.I4.Element.InitTooltip || function (id) {
    const el = document.getElementById(id);
    if (el == null) return;

    if (!el.hasOwnProperty('I4EBitTooltip')) {
        if (el.dataset.hasOwnProperty('i4e.tooltipText')) {
            el.I4EBitTooltip = tippy(el, {
                content: el.dataset['i4e.tooltipText'],
                followCursor: "initial",
                delay: [el.dataset['i4e.tooltipDelay'], 0],
                placement: el.dataset['i4e.tooltipPlacement']
            });
        }
    } else {
        if (el.dataset.hasOwnProperty('i4e.tooltipText')) {
            el.I4EBitTooltip.setContent(el.dataset['i4e.tooltipText']);
            el.I4EBitTooltip.enable();
        } else {
            el.I4EBitTooltip.disable();
        }
    }
}

//

window.I4.Element.DropdownShowEvents = ['mouseenter', 'focus'];
window.I4.Element.DropdownHideEvents = ['mouseleave', 'blur'];

window.I4.Element.InitDropdown = window.I4.Element.InitDropdown || function (toggle, contents) {
    if (!toggle.hasOwnProperty('I4EBitDropdown')) {
        toggle.I4EBitDropdown = Popper.createPopper(toggle, contents, {
            placement: contents.getAttribute('data-popper-placement'),
        });

        window.I4.Element.DropdownShowEvents.forEach(event => {
            toggle.addEventListener(event, () => {
                toggle.setAttribute('data-contents-shown', '');
                contents.setAttribute('data-show', '');

                toggle.I4EBitDropdown.setOptions({
                    modifiers: [{name: 'eventListeners', enabled: true}],
                });

                contents.style.minWidth = (toggle.clientWidth - 4) + 'px';

                toggle.I4EBitDropdown.update();
            });
        });

        window.I4.Element.DropdownHideEvents.forEach(event => {
            toggle.parentElement.addEventListener(event, () => {
                toggle.removeAttribute('data-contents-shown')
                contents.removeAttribute('data-show')
            });

            toggle.I4EBitDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: false}],
            });
        });
    }
}

//

window.I4.Element.HighlightPageLink = window.I4.Element.HighlightPageLink || function (id, highlighted) {
    const el = document.getElementById(id);
    if (el == null) return;

    if (highlighted) {
        el.dataset['highlighted'] = '';
        el.setAttribute('data-highlighted', '');
    } else {
        el.removeAttribute('data-highlighted')
    }
}
