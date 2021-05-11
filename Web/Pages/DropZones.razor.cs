using System;
using System.Threading.Tasks;
using Integrant4.Element.Constructs.FileUploader;

namespace Web.Pages
{
    public partial class DropZones : IAsyncDisposable
    {
        private FileUploader _dropZone1 = null!;
        private FileUploader _dropZone2 = null!;
        private FileUploader _dropZone3 = null!;
        private FileUploader _dropZone4 = null!;

        public async ValueTask DisposeAsync()
        {
            await _dropZone1.DisposeAsync();
            await _dropZone2.DisposeAsync();
            await _dropZone3.DisposeAsync();
            await _dropZone4.DisposeAsync();
        }

        protected override void OnInitialized()
        {
            _dropZone1 = new FileUploader(FileUploader.Type.Single   | FileUploader.Type.Block);
            _dropZone2 = new FileUploader(FileUploader.Type.Multiple | FileUploader.Type.Block);
            _dropZone3 = new FileUploader(FileUploader.Type.Single   | FileUploader.Type.Inline);
            _dropZone4 = new FileUploader(FileUploader.Type.Multiple | FileUploader.Type.Inline);

            _dropZone2.ActivatePasteHandler();
        }
    }
}