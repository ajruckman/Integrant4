window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.GetMarkdownEditorTheme = window.I4.Element.GetMarkdownEditorTheme || function () {
    if (window.I4.Colorant.ThemeVariant.hasOwnProperty('Main')) {
        let mainVariant = window.I4.Colorant.ThemeVariant['Main'];
        if (mainVariant === 'Dark' || mainVariant === 'Matrix') {
            return "dark";
        } else if (mainVariant === "White" || mainVariant === "Pink") {
            return undefined;
        }
    }

    return undefined;
}

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
    let buttonElems = [];

    for (let group of buttons) {
        let v = [];

        for (let button of group) {
            let b = document.createElement('button');

            b.className = 'I4E-Construct-MarkdownEditor-Button';
            b.innerHTML = '<span>' + button + '</span>';
            b.addEventListener('click', e => {
                editor.exec(button, e);
            });

            v.push({
                el: b,
                command: button,
            });
        }

        buttonElems.push(v);
    }

    const editor = new toastui.Editor({
        el: element,
        initialValue: value,
        placeholder: placeholder,
        toolbarItems: buttons,
        height: height + 'px',
        usageStatistics: false,
        theme: window.I4.Element.GetMarkdownEditorTheme(),

        events: {
            change: debounce(() => {
                objRef.invokeMethodAsync("Change", editor.getMarkdown());
            }, debounceMilliseconds),
        }
    });

    // Button overrides

    // let observer = new MutationObserver(function (m) {
    //     let closeButtons = element.querySelectorAll('button.toastui-editor-close-button');
    //     for (let v of closeButtons) {
    //         v.classList.remove('toastui-editor-close-button');
    //         v.classList.add('I4E-Bit-Button');
    //         v.classList.add('I4E-Bit-Button--Red');
    //     }
    //
    //     let okButtons = element.querySelectorAll('button.toastui-editor-ok-button');
    //     for (let v of okButtons) {
    //         v.classList.remove('toastui-editor-ok-button');
    //         v.classList.add('I4E-Bit-Button');
    //         v.classList.add('I4E-Bit-Button--Green');
    //     }
    //
    //     // let fsButtons = element.querySelectorAll('button.toastui-editor-file-select-button');
    //     // for (let v of fsButtons) {
    //     //     v.classList.remove('toastui-editor-file-select-button');
    //     //     v.classList.add('I4E-Bit-Button');
    //     // }
    // });

    // let popup = element.querySelector('div.toastui-editor-popup');
    // observer.observe(popup, {attributes: false, childList: true, characterData: false, subtree: true});
    
    // Theme overrides
    
    window.I4.Colorant.ThemeEvent.addEventListener('i4c_theme_change', function (e) {
        let ui = element.querySelector("div.toastui-editor-defaultUI");

        switch (editor.options.theme = window.I4.Element.GetMarkdownEditorTheme()) {
            case undefined:
                if (ui.classList.contains("toastui-editor-dark"))
                    ui.classList.remove("toastui-editor-dark");
                break;
            case "dark":
                if (!ui.classList.contains("toastui-editor-dark"))
                    ui.classList.add("toastui-editor-dark");
                break;
        }
    });

    //

    editor.I4EDisabled = isDisabled;

    editor.I4EUpdateState = function (placeholder, height, isDisabled) {
        editor.setPlaceholder(placeholder);
        editor.setHeight(height + 'px');
        editor.I4EDisabled = isDisabled;

    };

    editor.I4EGetMarkdown = function () {
        return editor.getMarkdown();
    }

    editor.I4ESetMarkdown = function (markdown) {
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
