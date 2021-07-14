window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.DropdownShowEvents = ["mouseenter", "focus"];
window.I4.Element.DropdownHideEvents = ["mouseleave", "blur"];

//

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
        sibling = sibling.nextElementSibling;
    }
    return null;
};

//

const ScrollbarSpace = '7px';

window.addEventListener("focusin", event => {
    if (document.I4EActiveElement === undefined) {
        document.I4EActiveElement = document.activeElement;
    } else {
        document.I4EActiveElementPrev = document.I4EActiveElement;
        document.I4EActiveElement = document.activeElement;
    }
})

window.I4.Element.InitSelector = window.I4.Element.InitSelector || function (element, dotnetHelper, filterable) {
    if (element == null) {
        console.log("Element reference passed to InitSelector is null; exiting");
        return;
    }

    const head = element.querySelector(".I4E-Construct-Selector-Head");
    const clearValueButton = element.querySelector(".I4E-Construct-Selector-Head span.I4R-BootstrapIcon");
    const dropdown = element.querySelector(".I4E-Construct-Selector-Dropdown");
    const filterInput = element.querySelector(".I4E-Input-Text input[type=text]");
    const scroller = element.querySelector(".I4E-Construct-Selector-Scroller");
    const options = element.querySelector(".I4E-Construct-Selector-Options");

    if (!element.hasOwnProperty("I4EOptionsDropdown")) {
        element.I4EOptionsDropdown = Popper.createPopper(head, dropdown, {
            placement: "bottom",

            modifiers: [{name: "offset", options: {offset: [0, 5]}}],
        });

        element.SelectorOpen = false;
        
        let glue;

        element.Scrollbar = OverlayScrollbars(scroller, {
            resize: "both",
        });

        element.ShowSelector = function () {
            element.SelectorOpen = true;

            element.setAttribute("data-open", "");
            head.setAttribute("data-focused", "");
            dropdown.setAttribute("data-open", "");

            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: "eventListeners", enabled: true}, {name: "offset", options: {offset: [0, 1]}}],
            });

            dropdown.style.minWidth = (head.clientWidth + 1) + "px";

            options.querySelector("div[data-selected]")?.focus();

            element.I4EOptionsDropdown.update();

            //

            element.Scrollbar.update();

            if (element.Scrollbar.getState().hasOverflow.y === true) {
                options.style.width = `calc(100% - ${ScrollbarSpace})`;
            } else {
                options.style.width = `100%`;
            }

            //

            if (filterable) {
                element.PrevFocus = document.I4EActiveElementPrev;
                filterInput.focus();
            }

            //

            dotnetHelper.invokeMethodAsync("I4E.Construct.Selector.Open");
        };

        element.HideSelector = function () {
            element.SelectorOpen = false;
            element.removeAttribute("data-open");
            head.removeAttribute("data-focused");
            dropdown.removeAttribute("data-open");

            window.setTimeout(function() {
                if (filterable && element.PrevFocus !== undefined) {
                    element.PrevFocus.focus();
                }
            }, 0);
            
            element.I4EOptionsDropdown.setOptions({
                modifiers: [{name: "eventListeners", enabled: false}, {name: "offset", options: {offset: [0, 1]}}],
            });
            
            element.Scrollbar.sleep();
        };

        element.Select = function (i) {
            dotnetHelper.invokeMethodAsync("I4E.Construct.Selector.Select", i);
            element.HideSelector();
            head.focus();
        };

        // Event listeners

        // Open dropdown on head click
        head.addEventListener("click", event => {
            if (head.hasAttribute("data-disabled")) {
                return;
            }

            if (!clearValueButton.contains(event.target) && !element.SelectorOpen) {
                head.focus();
                element.ShowSelector();
            }
        });

        // Head keydown handlers
        head.addEventListener("keydown", event => {
            if (head.hasAttribute("data-disabled")) {
                return;
            }

            // Toggle dropdown when head is focused and Space or Enter keys are pressed
            if (event.code === "Space" || event.code === "Enter") {
                event.preventDefault();

                if (!element.SelectorOpen) {
                    element.ShowSelector();
                } else {
                    element.HideSelector();
                }
            }

            // Focus the first option or filter input when head is focused and the Down key is pressed, if possible
            else if (event.code === "ArrowDown") {
                event.preventDefault();

                if (!element.SelectorOpen) {
                    element.ShowSelector();
                } else if (!filterable) {
                    options.querySelector(":scope > div")?.focus();
                } else {
                    filterInput.focus();
                }
            } else if (event.code !== "Escape" && element.SelectorOpen && filterable) {
                filterInput.focus();
            }
        });

        // Close dropdown when other area on page is clicked
        element.I4EMousedownEventListener = function (event) {
            if (!element.contains(event.target)) {
                element.HideSelector();
            }
        };
        document.addEventListener("mousedown", element.I4EMousedownEventListener);

        // Close dropdown when Escape key is pressed
        element.I4EKeydownEventListener = function (event) {
            if (event.code === "Escape" && element.SelectorOpen) {
                element.HideSelector();
            }
        };
        document.addEventListener("keydown", element.I4EKeydownEventListener);

        // Select value when option is clicked
        options.addEventListener("click", event => {
            let option = event.target.closest("div[data-i]");

            if (option.hasAttribute("data-disabled")) return;
            if (option.hasAttribute("data-placeholder")) return;
            element.Select(parseInt(option.getAttribute("data-i")));
        });

        // Options keydown handlers
        options.addEventListener("keydown", event => {
            // Close if the target is not a <div>; the dropdown uses <span>s for other text
            if (!(event.target.tagName === "DIV")) return;

            // Select the focused option when the Space or Enter keys are pressed
            if (event.code === "Space" || event.code === "Enter") {
                event.preventDefault();
                if (event.target.hasAttribute("data-disabled")) return;
                if (event.target.hasAttribute("data-placeholder")) return;
                element.Select(parseInt(event.target.getAttribute("data-i")));
            } else if (event.code === "ArrowUp") {
                event.preventDefault();

                let prev;

                if (!filterable) {
                    prev = window.I4.Element.PreviousSiblingSelector(event.target, "div");
                } else {
                    prev = window.I4.Element.PreviousSiblingSelector(event.target, "div[data-shown]");
                }

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
            else if (event.code === "ArrowDown") {
                event.preventDefault();

                if (!filterable) {
                    window.I4.Element.NextSiblingSelector(event.target, "div")?.focus();
                } else {
                    window.I4.Element.NextSiblingSelector(event.target, "div[data-shown]")?.focus();
                }
            }
        });

        // Focus the first option when input is focused and the Down key is pressed, if possible
        if (filterable) {
            filterInput.addEventListener("keydown", event => {
                if (event.code === "ArrowDown") {
                    event.preventDefault();
                    options.querySelector(":scope > div[data-shown]")?.focus();
                }
            });
        }
    }

    // element.ShowSelector();
};

window.I4.Element.ShowSelector = window.I4.Element.ShowSelector || function (element) {
    element.ShowSelector();
};

window.I4.Element.HideSelector = window.I4.Element.HideSelector || function (element) {
    element.HideSelector();
};

window.I4.Element.UpdateSelector = window.I4.Element.UpdateSelector || function (element) {
    element.I4EOptionsDropdown.update();
    element.Scrollbar.update();
};

window.I4.Element.DeactivateSelector = window.I4.Element.DeactivateSelector || function (element) {
    if (element.hasOwnProperty("I4EMousedownEventListener")) {
        document.removeEventListener("mousedown", element.I4EMousedownEventListener);
    }
    if (element.hasOwnProperty("I4EKeydownEventListener")) {
        document.removeEventListener("keydown", element.I4EKeydownEventListener);
    }
};

//

window.I4.Element.FileUploaderInit = window.I4.Element.FileUploaderInit || function (guid, element) {
    let input = element.querySelector("input");
    let indicator = element.querySelector(".I4E-Construct-FileUploader-DropIndicator");

    input.addEventListener("drop", event => {
        event.stopPropagation();
        event.preventDefault();
        indicator.style.display = "none";

        window.I4.Element.FileUploaderSendFiles(guid, event.dataTransfer.files);
    });

    input.addEventListener("change", event => {
        event.stopPropagation();
        event.preventDefault();
        indicator.style.display = "none";

        window.I4.Element.FileUploaderSendFiles(guid, event.target.files);
    });

    input.addEventListener("dragenter", () => {
        indicator.style.display = "block";
    });

    input.addEventListener("dragleave", () => {
        indicator.style.display = "none";
    });
};

window.I4.Element.FileUploaderActivatePasteHandler = window.I4.Element.FileUploaderActivatePasteHandler || function (guid, element) {
    element.I4EPasteHandler = function I4EPasteHandler(e) {
        const items = (e.clipboardData || e.originalEvent.clipboardData.items);

        let files = [];
        for (let i = 0; i < items.files.length; i++) {
            let file = items.files[i];
            files.push(file);
        }

        window.I4.Element.FileUploaderSendFiles(guid, files);
    };

    window.addEventListener("paste", element.I4EPasteHandler);
};

window.I4.Element.FileUploaderDeactivatePasteHandler = window.I4.Element.FileUploaderDeactivatePasteHandler || function (element) {
    window.removeEventListener("paste", element.I4EPasteHandler);
};

window.I4.Element.FileUploaderSendFiles = window.I4.Element.FileUploaderSendFiles || function (guid, files) {
    let data = new FormData();
    data.append("guid", guid);

    Array.from(files).forEach(file => {
        data.append("file", file);
    });

    fetch("/api/i4/file", {
        method: "POST",
        body: data,
    }).then(() => {
    }).catch(e => {
        console.log("Failed to upload file.");
        console.log(e);
    });
};

const SVGNS = "http://www.w3.org/2000/svg";
const XLNNS = "http://www.w3.org/1999/xlink";

window.I4.Element.FileUploaderThumbnailErrorHandler = window.I4.Element.FileUploaderThumbnailErrorHandler || function (self) {
    let svg = document.createElementNS(SVGNS, "svg");
    let use = document.createElementNS(SVGNS, "use");

    svg.setAttribute("class", "bi");
    svg.setAttribute("width", "64");
    svg.setAttribute("height", "64");
    use.setAttributeNS(XLNNS, "href", "_content/Integrant4.Resources/Icons/Bootstrap/bootstrap-icons.svg#file-earmark");

    svg.appendChild(use);
    self.replaceWith(svg);
};
