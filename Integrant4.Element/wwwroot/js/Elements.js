window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

//

window.I4.Element.InitTooltip = window.I4.Element.InitTooltip || function (id) {
    const el = document.getElementById(id);
    if (el == null) return;

    if (!el.hasOwnProperty('I4EBitTooltip')) {
        if (el.dataset.hasOwnProperty('i4e.tooltipText')) {
            el.I4EBitTooltip = tippy(el, {
                allowHTML: true,
                content: el.dataset['i4e.tooltipText'],
                followCursor: el.dataset['i4e.tooltipFollow'],
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

window.I4.Element.InitDropdown = window.I4.Element.InitDropdown || function (head, children) {
    if (head == null) {
        console.log("Head passed to InitDropdown is null; exiting")
        return;
    }

    if (!head.hasOwnProperty('I4EBitDropdown')) {
        head.I4EBitDropdown = Popper.createPopper(head, children, {
            placement: children.getAttribute('data-popper-placement'),
        });

        function show() {
            head.setAttribute('data-children-open', '');
            children.setAttribute('data-show', '');

            head.I4EBitDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: true}],
            });

            children.style.minWidth = (head.clientWidth - 4) + 'px';

            head.I4EBitDropdown.update();
        }

        function hide() {
            head.removeAttribute('data-children-open');
            children.removeAttribute('data-show');

            head.I4EBitDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: false}],
            });
        }

        window.I4.Element.DropdownShowEvents.forEach(event => {
            head.addEventListener(event, show);
        });

        window.I4.Element.DropdownHideEvents.forEach(event => {
            head.parentElement.addEventListener(event, hide);
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