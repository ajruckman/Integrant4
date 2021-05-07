using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Integrant4.Element.Constructs.FileUploader
{
    [Route("api/i4/file")]
    [ApiController]
    public class FileUploaderController : ControllerBase
    {
        private const int ThumbnailSize = 96;

        private readonly FileUploaderService _fileUploaderService;

        public FileUploaderController(FileUploaderService fileUploaderService)
        {
            this._fileUploaderService = fileUploaderService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            Guid guid = Guid.Parse(Request.Form["guid"][0]);

            foreach (IFormFile formFile in Request.Form.Files)
            {
                Console.WriteLine($"GOT: {formFile.FileName}");

                var data = new byte[formFile.Length];

                await using Stream stream = formFile.OpenReadStream();
                await stream.ReadAsync(data);
                MemoryStream memoryStream = new(data);

                _fileUploaderService.Add(guid, formFile.FileName, memoryStream);
            }

            return Ok(new {success = true});
        }

        [HttpGet]
        [Route("thumbnail/{GUID:guid}/{ID:int}")]
        public IActionResult GetThumbnail(Guid guid, int id)
        {
            FileUploader.File? file = _fileUploaderService.Get(guid, id);
            if (file == null)
                return new EmptyResult();

            try
            {
                Image image = Image.FromStream(file.Data);

                int h  = image.Height;
                int w  = image.Width;
                var hf = (float) h;
                var wf = (float) w;

                if (h > w)
                {
                    w = (int) (wf / hf * ThumbnailSize);
                    h = ThumbnailSize;
                }
                else
                {
                    h = (int) (hf / wf * ThumbnailSize);
                    w = ThumbnailSize;
                }

                Image thumb = image.GetThumbnailImage
                (
                    w, h,
                    () => false,
                    IntPtr.Zero
                );

                MemoryStream res = new();
                thumb.Save(res, ImageFormat.Png);
                res.Seek(0, SeekOrigin.Begin);

                return File(res, "image/png");
            }
            catch
            {
                return new EmptyResult();
            }
        }
    }
}