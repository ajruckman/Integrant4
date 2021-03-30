window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.DropdownShowEvents = ['mouseenter', 'focus'];
window.I4.Element.DropdownHideEvents = ['mouseleave', 'blur'];

window.I4.Element.OptionsShowEvents = ['click'];
window.I4.Element.OptionsHideEvents = ['mousedown'];

//

window.I4.Element.InitSelector = window.I4.Element.InitSelector || function (element, dotnetHelper) {
    console.log("Init selector:");
    console.log(element);

    if (element == null) {
        console.log("Element reference passed to InitSelector is null; exiting")
        return;
    }

    const head = element.querySelector('.I4E-Construct-Selector-Head');
    const dropdown = element.querySelector('.I4E-Construct-Selector-Dropdown');
    const filterInput = element.querySelector('input[type=text]');
    const options = element.querySelector('.I4E-Construct-Selector-Options');

    console.log(head)
    console.log(dropdown)
    console.log(filterInput)
    console.log(options)

    if (!element.hasOwnProperty('I4EOptionsDropdown')) {
        element.I4EOptionsDropdown = Popper.createPopper(head, dropdown, {
            placement: 'bottom',
        });

        element.ShowSelector = function show() {
            element.setAttribute('data-dropdown-shown', '');
            dropdown.setAttribute('data-show', '');

            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: true}],
            });

            dropdown.parentElement.style.minWidth = (head.clientWidth - 4) + 'px';

            head.parentElement.I4EOptionsDropdown.update();
        }

        element.HideSelector = function hide() {
            element.removeAttribute('data-dropdown-shown');
            dropdown.removeAttribute('data-show');

            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: false}],
            });
        }

        window.I4.Element.OptionsShowEvents.forEach(event => {
            head.addEventListener(event, element.ShowSelector);
        });

        window.I4.Element.OptionsHideEvents.forEach(event => {
            document.addEventListener(event, event => {
                if (!element.contains(event.target)) {
                    element.HideSelector();
                }
            });
        });

        //

        filterInput.addEventListener('input', event => {
            const filter = filterInput.value.trim().toUpperCase();
            const children = options.getElementsByTagName('div');

            let child, textValue;

            if (filter === '') {
                for (let i = 0; i < children.length; i++) {
                    children[i].style.display = '';
                }
            } else {
                for (let i = 0; i < children.length; i++) {
                    child = children[i];
                    textValue = child.textContent || child.innerText;
                    if (textValue.toUpperCase().indexOf(filter) > -1) {
                        child.style.display = '';
                    } else {
                        child.style.display = 'none';
                    }
                }
            }
        });
    }

    options.addEventListener('click', event => {
        console.log(event);
        console.log(event.target);
        console.log(event.target.getAttribute('i'));

        dotnetHelper.invokeMethodAsync
        (
            'I4E.Construct.Selector.Select',
            parseInt(event.target.getAttribute('i'))
        );

        head.parentElement.HideSelector();
    })
}

window.I4.Element.ShowSelector = window.I4.Element.ShowSelector || function (head) {
    head.parentElement.ShowSelector();
}

window.I4.Element.HideSelector = window.I4.Element.HideSelector || function (head) {
    head.parentElement.HideSelector();
}

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
            head.setAttribute('data-children-shown', '');
            children.setAttribute('data-show', '');

            head.I4EBitDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: true}],
            });

            children.style.minWidth = (head.clientWidth - 4) + 'px';

            head.I4EBitDropdown.update();
        }

        function hide() {
            head.removeAttribute('data-children-shown');
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
