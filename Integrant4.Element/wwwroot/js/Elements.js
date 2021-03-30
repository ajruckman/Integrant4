window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.DropdownShowEvents = ['mouseenter', 'focus'];
window.I4.Element.DropdownHideEvents = ['mouseleave', 'blur'];

window.I4.Element.PreviousSiblingSelector = window.I4.Element.PreviousSiblingSelector || function (element, selector) {
    let sibling = element.previousElementSibling;
    while (sibling) {
        if (sibling.matches(selector)) return sibling;
        sibling = sibling.previousElementSibling;
    }
    return null;
};

window.I4.Element.NextSiblingSelector = window.I4.Element.NextSiblingSelector || function (element, selector) {
    let sibling = element.nextElementSibling;
    while (sibling) {
        if (sibling.matches(selector)) return sibling;
        sibling = sibling.nextElementSibling
    }
    return null;
};

//

window.I4.Element.InitSelector = window.I4.Element.InitSelector || function (element, dotnetHelper, filterable) {
    console.log("Init selector:");
    console.log(element);

    if (element == null) {
        console.log("Element reference passed to InitSelector is null; exiting")
        return;
    }

    const head = element.querySelector('.I4E-Construct-Selector-Head');
    const clearValueButton = element.querySelector('.I4E-Construct-Selector-Head button');
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

            modifiers: [{name: 'offset', options: {offset: [0, 5]}}],
        });

        element.SelectorShown = false;

        element.ShowSelector = function () {
            element.SelectorShown = true;
            element.setAttribute('data-dropdown-shown', '');
            dropdown.setAttribute('data-shown', '');

            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: true}, {name: 'offset', options: {offset: [0, 1]}}],
            });

            dropdown.style.width = (head.clientWidth) + 'px';
            
            options.querySelector('div[data-selected]')?.focus();

            element.I4EOptionsDropdown.update();
        };

        element.HideSelector = function () {
            element.SelectorShown = false;
            element.removeAttribute('data-dropdown-shown');
            dropdown.removeAttribute('data-shown');

            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: 'eventListeners', enabled: false}, {name: 'offset', options: {offset: [0, 1]}}],
            });
        };

        element.Select = function (i) {
            dotnetHelper.invokeMethodAsync('I4E.Construct.Selector.Select', i);
            element.HideSelector();
            head.focus();
        };

        // Event listeners

        // Open dropdown on head click
        head.addEventListener('click', event => {
            if (!clearValueButton.contains(event.target) && !element.SelectorShown) {
                head.focus();
                element.ShowSelector();
            }
        });

        // Head keydown handlers
        head.addEventListener('keydown', event => {
            console.log(event)
            console.log(event.code)
            // Toggle dropdown when head is focused and Space or Enter keys are pressed
            if (event.code === 'Space' || event.code === 'Enter') {
                event.preventDefault();

                if (!element.SelectorShown) {
                    element.ShowSelector();
                } else {
                    element.HideSelector();
                }
            }
            // Focus the first option or filter input when head is focused and the Down key is pressed, if possible
            else if (event.code === 'ArrowDown') {
                event.preventDefault();

                if (!element.SelectorShown) {
                    element.ShowSelector();
                } else if (!filterable) {
                    options.querySelector(':scope > div')?.focus();
                } else {
                    filterInput.focus();
                }
            }
        });

        // Close dropdown when other area on page is clicked
        document.addEventListener('mousedown', event => {
            if (!element.contains(event.target)) {
                element.HideSelector();
            }
        });

        // Close dropdown when Escape key is pressed
        document.addEventListener('keydown', event => {
            if (event.code === 'Escape' && element.SelectorShown) {
                element.HideSelector();
            }
        });

        // Select value when option is clicked
        options.addEventListener('click', event => {
            if (event.target.tagName === 'DIV') {
                element.Select(parseInt(event.target.getAttribute('data-i')));
            }
        });

        // Options keydown handlers
        options.addEventListener('keydown', event => {
            console.log(event)
            console.log(event.code)

            // Close if the target is not a <div>; the dropdown uses <span>s for other text
            if (!(event.target.tagName === 'DIV')) return;

            // Select the focused option when the Space or Enter keys are pressed
            if (event.code === 'Space' || event.code === 'Enter') {
                event.preventDefault();
                element.Select(parseInt(event.target.getAttribute('data-i')));
            } else if (event.code === 'ArrowUp') {
                event.preventDefault();

                let prev;

                if (!filterable) {
                    console.log('!')
                    prev = window.I4.Element.PreviousSiblingSelector(event.target, 'div');
                } else {
                    prev = window.I4.Element.PreviousSiblingSelector(event.target, 'div[data-shown]');
                }

                console.log(event.target)
                console.log(prev);

                // Focus the previous option if there is one
                if (prev != null) {
                    prev.focus();
                }
                // Otherwise, this is the first option in the list, and is filterable, so focus the filter input
                else if (filterable) {
                    filterInput.focus();
                }
            }
            // Focus the next option if there is one
            else if (event.code === 'ArrowDown') {
                event.preventDefault();

                if (!filterable) {
                    window.I4.Element.NextSiblingSelector(event.target, 'div')?.focus();
                } else {
                    window.I4.Element.NextSiblingSelector(event.target, 'div[data-shown]')?.focus();
                }
            }
        });

        // Focus the first option when input is focused and the Down key is pressed, if possible
        if (filterable) {
            filterInput.addEventListener('keydown', event => {
                if (event.code === 'ArrowDown') {
                    event.preventDefault();
                    options.querySelector(':scope > div[data-shown]')?.focus();
                }
            });
        }


        //

        // filterInput.addEventListener('input', event => {
        //     const filter = filterInput.value.trim().toUpperCase();
        //     const children = options.getElementsByTagName('div');
        //
        //     let child, textValue;
        //
        //     if (filter === '') {
        //         for (let i = 0; i < children.length; i++) {
        //             children[i].style.display = '';
        //         }
        //     } else {
        //         for (let i = 0; i < children.length; i++) {
        //             child = children[i];
        //             textValue = child.textContent || child.innerText;
        //             if (textValue.toUpperCase().indexOf(filter) > -1) {
        //                 child.style.display = '';
        //             } else {
        //                 child.style.display = 'none';
        //             }
        //         }
        //     }
        // });
    }

    element.ShowSelector();
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
