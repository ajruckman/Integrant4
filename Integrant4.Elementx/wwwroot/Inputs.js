window.I4 = window.I4 || {};
window.I4.Element = window.I4.Element || {};
window.I4.Element.Inputs = window.I4.Element.Inputs || {};

window.I4.Element.Inputs.GetValue = window.I4.Element.Inputs.GetValue || function (ref) {
    return ref.value;
};

window.I4.Element.Inputs.SetValue = window.I4.Element.Inputs.SetValue || function (ref, value) {
    return ref.value = value;
};

window.I4.Element.Inputs.SetDisabled = window.I4.Element.Inputs.SetDisabled || function (ref, disabled) {
    return ref.disabled = disabled;
};

window.I4.Element.Inputs.SetRequired = window.I4.Element.Inputs.SetDisabled || function (ref, required) {
    return ref.required = required;
};

window.I4.Element.Inputs.SetPlaceholder = window.I4.Element.Inputs.SetPlaceholder || function (ref, placeholder) {
    return ref.placeholder = placeholder;
};

//

window.I4.Element.Inputs.GetChecked = window.I4.Element.Inputs.GetChecked || function (ref) {
    return ref.checked ? 'True' : 'False';
};

window.I4.Element.Inputs.SetChecked = window.I4.Element.Inputs.SetChecked || function (ref, checked) {
    return ref.checked = checked;
};
