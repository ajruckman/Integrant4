using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.FileUploader;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class FileInput : IRefreshableInput<IReadOnlyList<File>?>, IAsyncDisposable
    {
        private readonly FileUploader _fileInput;

        public FileInput(FileUploader fileInput)
        {
            _fileInput = fileInput;

            _fileInput.OnChange += v => OnChange?.Invoke(v);
        }

        public async ValueTask DisposeAsync()
        {
            await _fileInput.DisposeAsync();
        }

        public void                       Refresh()   => _fileInput.Refresh();
        public RenderFragment             Renderer()  => _fileInput.Renderer();
        public IReadOnlyList<File>?       GetValue()  => _fileInput.GetValue();
        public Task<IReadOnlyList<File>?> ReadValue() => Task.FromResult(GetValue());

        public event Action<IReadOnlyList<File>?>? OnChange;

        public Task ActivatePasteHandler() => _fileInput.ActivatePasteHandler();
    }
}