window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};

//

window.I4.Element.InitChart = window.I4.Element.InitChart || function (elem, opts, data) {
    console.log(elem)
    console.log(opts)
    console.log('-')
    console.log(data)
    console.log(typeof(data))
    console.log(Array.isArray(data))
    
    if (elem.I4EChart == null) {
        if (data == null) {
            return;
        }

        elem.I4EChart = new uPlot(opts, data, elem);

        setTimeout(() => {
            elem.I4EChart.setSize({width: elem.clientWidth, height: opts.height})
        })
    } else {
        console.log("update")
        elem.I4EChart.setData(data);
    }
};
