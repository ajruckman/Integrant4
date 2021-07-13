using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.MarkdownEditor;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class MarkdownInput : IWritableRefreshableInput<string?>
    {
        private readonly MarkdownEditor _editor;

        public MarkdownInput(MarkdownEditor editor)
        {
            _editor = editor;

            _editor.OnChange += v => OnChange?.Invoke(v);
        }

        public async Task SetValue(string? value, bool invokeOnChange = true) => await _editor.SetValue(value);

        public void Refresh() => _editor.Refresh();

        public RenderFragment Renderer() => _editor.Renderer();

        public Task<string?> GetValue() => Task.FromResult(_editor.GetValue());

        public event Action<string?>? OnChange;
    }
}