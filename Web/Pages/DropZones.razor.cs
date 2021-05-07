using Integrant4.Element.Constructs;
using Integrant4.Element.Constructs.FileUploader;

namespace Web.Pages
{
    public partial class DropZones
    {
        private FileUploader _dropZone1 = null!;

        protected override void OnInitialized()
        {
            _dropZone1 = new FileUploader();
        }
    }
}