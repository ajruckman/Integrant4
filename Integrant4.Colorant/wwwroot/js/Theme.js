window.I4 = window.I4 || {};
window.I4.Colorant = window.I4.Colorant || {};

window.I4.Colorant.ThemeEvent = window.I4.Colorant.ThemeEvent || document.createElement('meta');
window.I4.Colorant.ThemeVariant = window.I4.Colorant.ThemeVariant || {};

window.I4.Colorant.SetThemeVariant = window.I4.Colorant.SetThemeVariant || function (t, v) {
    window.I4.Colorant.ThemeVariant[t] = v;

    const event = new Event('i4c_theme_change');
    window.I4.Colorant.ThemeEvent.dispatchEvent(event);
}
