window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

window.I4.Element.InitTooltip = window.I4.Element.InitTooltip || function (el) {
    if (!el.hasOwnProperty('I4EBitTooltip')) {
        if (el.dataset.hasOwnProperty('i4e.tooltip')) {
            el.I4EBitTooltip = tippy(el, {
                content: el.dataset['i4e.tooltip'],
                followCursor: "initial",
            });
        }
    } else {
        if (el.dataset.hasOwnProperty('i4e.tooltip')) {
            el.I4EBitTooltip.setContent(el.dataset['i4e.tooltip']);
            el.I4EBitTooltip.enable();
        } else {
            el.I4EBitTooltip.disable();
        }
    }
}

window.I4.Element.CreateBitTooltips = window.I4.Element.CreateBitTooltips || function (id) {
    const bit = document.getElementById(id);

    window.I4.Element.InitTooltip(bit);
    
    // const bits = document.querySelectorAll('[class~="I4E-Bit"]');
    //
    // function createTooltip(v) {
    //     if (!v.hasOwnProperty('I4EBitTooltip')) {
    //         if (v.dataset.hasOwnProperty('i4e.tooltip')) {
    //             v.I4EBitTooltip = tippy(v, {
    //                 content: v.dataset['i4e.tooltip'],
    //                 followCursor: "initial",
    //             });
    //         }
    //     } else {
    //         if (v.dataset.hasOwnProperty('i4e.tooltip')) {
    //             v.I4EBitTooltip.setContent(v.dataset['i4e.tooltip']);
    //             v.I4EBitTooltip.enable();
    //         } else {
    //             v.I4EBitTooltip.disable();
    //         }
    //     }
    // }
    //
    // const tooltipObserver = new MutationObserver((r) => {
    //     for (const m of r) {
    //         if (m.type === 'attributes' && m.attributeName === 'i4e.tooltip') {
    //             createTooltip(m.target);
    //         }
    //     }
    // });
    //
    // bits.forEach(v => {
    //     tooltipObserver.observe(v, {
    //         attributes: true,
    //         attributeFilter: ['i4e.tooltip'],
    //     });
    //
    //     createTooltip(v);
    // });
}
