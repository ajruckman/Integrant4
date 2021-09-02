window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

//

window.I4.Element.InitTooltip = window.I4.Element.InitTooltip || function (id) {
    const el = document.getElementById(id);
    if (el == null) return;

    if (!el.hasOwnProperty("I4EBitTooltip")) {
        if (el.dataset.hasOwnProperty("i4e.tooltipText")) {
            el.I4EBitTooltip = tippy(el, {
                allowHTML: true,
                content: el.dataset["i4e.tooltipText"],
                followCursor: el.dataset["i4e.tooltipFollow"],
                delay: [el.dataset["i4e.tooltipDelay"], 0],
                placement: el.dataset["i4e.tooltipPlacement"],
            });
        }
    } else {
        if (el.dataset.hasOwnProperty("i4e.tooltipText")) {
            el.I4EBitTooltip.setContent(el.dataset["i4e.tooltipText"]);
            el.I4EBitTooltip.enable();
        } else {
            el.I4EBitTooltip.disable();
        }
    }
};

//

window.I4.Element.InitDropdown = window.I4.Element.InitDropdown || function (head, contents, offset) {
    if (head == null) {
        console.log("Head passed to InitDropdown is null; exiting");
        return;
    }

    console.log(head);

    if (!head.hasOwnProperty("I4EBitDropdown")) {
        
        function genOps(shown) {
            let opts = {
                placement: head.getAttribute("data-popper-placement"),
                modifiers: [],
            };

            opts.modifiers.push({name: "eventListeners", enabled: shown});

            let offset = head.getAttribute("data-popper-offset");
            if (offset != null) {
                opts.modifiers.push({
                    name: "offset",
                    options: {
                        offset: [0, parseInt(offset)],
                    },
                })
            }
            
            return opts;
        }

        head.I4EBitDropdown = Popper.createPopper(head, contents, genOps(false));

        function show() {
            head.setAttribute("data-contents-open", "");
            contents.setAttribute("data-show", "");

            head.I4EBitDropdown.setOptions(genOps(true));

            console.log(head.I4EBitDropdown.state.options.modifiers);

            contents.style.minWidth = (head.clientWidth - 4) + "px";

            head.I4EBitDropdown.update();
        }

        function hide() {
            head.removeAttribute("data-contents-open");
            contents.removeAttribute("data-show");

            head.I4EBitDropdown.setOptions(genOps(false));

            console.log(head.I4EBitDropdown.state.options.modifiers);
        }

        window.I4.Element.DropdownShowEvents.forEach(event => {
            head.addEventListener(event, show);
        });

        window.I4.Element.DropdownHideEvents.forEach(event => {
            head.parentElement.addEventListener(event, hide);
        });
    }
};

//

window.I4.Element.HighlightHeaderLink = window.I4.Element.HighlightHeaderLink || function (id, highlighted) {
    const el = document.getElementById(id);
    if (el == null) return;

    if (highlighted) {
        el.dataset["highlighted"] = "";
        el.setAttribute("data-highlighted", "");
    } else {
        el.removeAttribute("data-highlighted");
    }
};