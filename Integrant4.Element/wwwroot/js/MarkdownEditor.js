window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.InitMarkdownEditor = window.I4.Element.InitMarkdownEditor || function (
    objRef,
    element,
    buttons,
    debounceMilliseconds,
    value,
    placeholder,
    height,
    isDisabled
) {
    console.log(buttons)
    const editor = new toastui.Editor({
        el: element,
        initialValue: value,
        placeholder: placeholder,
        toolbarItems: buttons,
        height: height + 'px',
        // width: width + 'px',
        usageStatistics: false,

        events: {
            change: debounce(() => {
                objRef.invokeMethodAsync("Change", editor.getMarkdown());
            }, debounceMilliseconds),
        }
    });

    editor.I4EDisabled = isDisabled;

    editor.I4EUpdateState = function(placeholder, height, isDisabled) {
        editor.setPlaceholder(placeholder);
        editor.setHeight(height + 'px');
        editor.I4EDisabled = isDisabled;
    };
    
    editor.I4EGetMarkdown = function() {
        return editor.getMarkdown();
    }
    
    editor.I4ESetMarkdown = function(markdown) {
        editor.setMarkdown(markdown);
    }

    element.addEventListener('keypress', e => {
        if (editor.I4EDisabled) {
            e.preventDefault();
            return false;
        }
    });

    element.addEventListener('click', e => {
        if (editor.I4EDisabled) {
            e.preventDefault();
            return false;
        }
    });
    
    return editor;
}

function debounce(func, timeout) {
    let t;

    return (...args) => {
        clearTimeout(t);
        t = setTimeout(() => {
            func.apply(this, args);
        }, timeout);
    }
}
