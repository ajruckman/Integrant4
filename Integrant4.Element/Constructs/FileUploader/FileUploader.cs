using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.FileUploader
{
    public partial class FileUploader : IConstruct, IAsyncDisposable
    {
        private readonly Type _type;
        private readonly Spec _spec;
        private readonly Guid _guid = Guid.NewGuid();

        private ElementService?      _elementService;
        private FileUploaderService? _fileUploaderService;

        private WriteOnlyHook?    _refresher;
        private ElementReference? _elementRef;

        public FileUploader(Type type = Type.Multiple | Type.Block, Spec? spec = null)
        {
            _type = type;
            _spec = spec ?? new Spec();

            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort) (12 * _spec.Scale?.Invoke() ?? 12));
        }

        public async ValueTask DisposeAsync()
        {
            _fileUploaderService?.Unsubscribe(_guid);
            await DeactivatePasteHandler();
        }

        private void OnAdd(File file)
        {
            _refresher?.Invoke();
            OnChange?.Invoke(GetValue());
        }

        private void OnRemove(File file)
        {
            _refresher?.Invoke();
            OnChange?.Invoke(GetValue());
        }
    }

    public partial class FileUploader
    {
        [Flags]
        public enum Type
        {
            Single = 1, Multiple = 2, Block = 4, Inline = 8,
        }

        public class Spec
        {
            public ContentRef?      PlaceholderContent { get; init; }
            public ContentRef?      SizeLimitContent   { get; init; }
            public Callbacks.Unit?  Width              { get; init; }
            public Callbacks.Scale? Scale              { get; init; }
        }
    }

    public partial class FileUploader
    {
        public void Refresh() => _refresher?.Invoke();

        public IReadOnlyList<File>? GetValue()
        {
            IReadOnlyList<File>? files = _fileUploaderService?.List(_guid);
            return files?.Count == 0 ? null : files;
        }

        public void Clear()
        {
            _fileUploaderService?.Clear(_guid);
            _refresher?.Invoke();
            OnChange?.Invoke(null);
        }

        public event Action<IReadOnlyList<File>?>? OnChange;
    }

    public partial class FileUploader
    {
        private bool _isPasteHandler;
        private bool _hasActivatedPastHandler;

        public async Task ActivatePasteHandler()
        {
            _isPasteHandler = true;
            await CallActivatePasteHandler();
        }

        private async Task CallActivatePasteHandler()
        {
            if (_hasActivatedPastHandler) return;
            if (_elementService == null || _elementRef == null) return;
            await _elementService.JSInvokeVoidAsync
            (
                "I4.Element.FileUploaderActivatePasteHandler", _guid, _elementRef
            );
            _hasActivatedPastHandler = true;
        }

        public async Task DeactivatePasteHandler()
        {
            if (!_isPasteHandler        || !_hasActivatedPastHandler) return;
            if (_elementService == null || _elementRef == null) return;
            await _elementService!.JSInvokeVoidAsync
            (
                "I4.Element.FileUploaderDeactivatePasteHandler", _elementRef
            );
            _hasActivatedPastHandler = false;
            _isPasteHandler          = false;
        }
    }

    public partial class FileUploader
    {
        private const string FallbackImage = "/_content/Integrant4.Resources/Icons/Bootstrap/file-earmark.svg";

        private readonly BootstrapIcon _deselectValueButton;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                Unit? width = _spec.Width?.Invoke();

                int seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService           = v);
                ServiceInjector<FileUploaderService>.Inject(builder, ref seq, v => _fileUploaderService = v);

                //

                builder.OpenElement(++seq, "div");

                string[] classes = new string[3];
                classes[0] = "I4E-Construct-FileUploader";
                classes[1] = "I4E-Construct-FileUploader--" + (_type.HasFlag(Type.Single) ? "Single" : "Multiple");
                classes[2] = "I4E-Construct-FileUploader--" + (_type.HasFlag(Type.Block) ? "Block" : "Inline");
                builder.AddAttribute(++seq, "class", string.Join(' ', classes));

                ++seq;
                if (width != null)
                    builder.AddAttribute(seq, "style", $"width: 100%; max-width: {width.Value.Serialize()};");

                builder.AddElementReferenceCapture(++seq, r => _elementRef = r);

                // Input

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type",     "file");
                builder.AddAttribute(++seq, "multiple", _type.HasFlag(Type.Multiple));
                builder.CloseElement();

                // Indicator

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-DropIndicator");
                builder.CloseElement();

                // Preview area

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-Region");

                IReadOnlyList<File>? list = _fileUploaderService?.List(_guid);

                if (list?.Count > 0)
                {
                    foreach (File file in list)
                    {
                        builder.OpenRegion(++seq);
                        var seqI = 0;

                        builder.OpenElement(++seqI, "div");
                        builder.AddAttribute(++seqI, "class", "I4E-Construct-FileUploader-File");

                        builder.SetKey(file.SerialID);
                        builder.OpenElement(++seqI, "div");
                        builder.OpenElement(++seqI, "img");
                        builder.AddAttribute(++seqI, "src", $"/api/i4/file/thumbnail/{_guid}/{file.SerialID}");
                        builder.AddAttribute(++seq, "onerror",
                            "window.I4.Element.FileUploaderThumbnailErrorHandler(this)");
                        builder.CloseElement();
                        builder.CloseElement();

                        builder.OpenElement(++seqI, "p");
                        builder.AddContent(++seqI, file.Name);
                        builder.CloseElement();

                        builder.OpenElement(++seqI, "span");
                        builder.AddAttribute(++seqI, "class",    "I4E-Construct-FileUploader-RemoveButtonWrapper");
                        builder.AddAttribute(++seqI, "tabindex", 0);
                        builder.AddAttribute(++seqI, "onclick",
                            EventCallback.Factory.Create(this,
                                () => _fileUploaderService?.Remove(_guid, file.SerialID)));
                        builder.AddContent(++seqI, _deselectValueButton.Renderer());
                        builder.CloseElement();

                        builder.CloseElement();

                        builder.CloseRegion();
                    }
                }
                else
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-EmptyContent");

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-PlaceholderContent");

                    IRenderable? text = _spec.PlaceholderContent?.GetOne();
                    if (text == null)
                    {
                        string def = _type.HasFlag(Type.Single)
                            ? "Drag and drop a file here,<br>or click to select one from your computer"
                            : "Drag and drop files here,<br>or click to select from your computer";
                        builder.AddMarkupContent(++seq, def);
                    }
                    else
                    {
                        builder.AddContent(++seq, text.Renderer());
                    }

                    builder.CloseElement();

                    //

                    IRenderable? sizeLimitContent = _spec.SizeLimitContent?.GetOne();
                    if (sizeLimitContent != null)
                    {
                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-SizeLimitContent");
                        builder.AddContent(++seq, sizeLimitContent.Renderer());
                        builder.CloseElement();
                    }

                    builder.CloseElement();
                }

                builder.CloseElement();

                builder.CloseElement();
            },
            v => _refresher = v,
            async firstRender =>
            {
                if (!firstRender) return;

                if (_elementService == null || _fileUploaderService == null || _elementRef == null)
                {
                    Console.WriteLine(
                        "Not initializing FileUploader because either a required service failed to inject " +
                        "or the drop region's element reference was not captured.");
                    return;
                }

                _fileUploaderService.Subscribe(_guid, _type.HasFlag(Type.Multiple), OnAdd, OnRemove);

                await _elementService.JSInvokeVoidAsync
                (
                    "I4.Element.FileUploaderInit", _guid, _elementRef
                );

                if (_isPasteHandler && !_hasActivatedPastHandler)
                {
                    await CallActivatePasteHandler();
                }
            });
    }
}