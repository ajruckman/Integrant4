using Integrant4.Element.Constructs;
using Integrant4.Element.Constructs.FileUploader;

namespace Web.Pages
{
    public partial class DropZones
    {
        private FileUploader _dropZone1 = null!;
        private FileUploader _dropZone2 = null!;
        private FileUploader _dropZone3 = null!;
        private FileUploader _dropZone4 = null!;

        protected override void OnInitialized()
        {
            _dropZone1 = new FileUploader(FileUploader.Type.Single   |FileUploader.Type.Block);
            _dropZone2 = new FileUploader(FileUploader.Type.Multiple |FileUploader.Type.Block);
            _dropZone3 = new FileUploader(FileUploader.Type.Single   |FileUploader.Type.Inline);
            _dropZone4 = new FileUploader(FileUploader.Type.Multiple |FileUploader.Type.Inline);
        }
    }
}