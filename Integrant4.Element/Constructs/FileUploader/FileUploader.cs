using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.FileUploader
{
    public partial class FileUploader : IConstruct, IDisposable
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

        public void Dispose()
        {
            _fileUploaderService?.Unsubscribe(_guid);
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

        public void Refresh() => _refresher?.Invoke();

        public IReadOnlyList<File>? GetValue()
        {
            IReadOnlyList<File>? files = _fileUploaderService?.List(_guid);
            return files?.Count == 0 ? null : files;
        }

        public class File
        {
            internal readonly ushort SerialID;

            public readonly string       Name;
            public readonly MemoryStream Data;

            internal File(ushort serialID, string name, MemoryStream data)
            {
                SerialID = serialID;
                Name     = name;
                Data     = data;
            }
        }

        public event Action<IReadOnlyList<File>?>? OnChange;
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
            [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
            [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]

            public Callbacks.Callback<string>? PlaceholderText { get; init; }

            public Callbacks.Pixels? Width { get; init; }
            public Callbacks.Scale?  Scale { get; init; }
        }
    }

    public partial class FileUploader
    {
        private readonly BootstrapIcon _deselectValueButton;

        private const string FallbackImage = "/_content/Integrant4.Resources/Icons/Bootstrap/file-earmark.svg";

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                double? width = _spec.Width?.Invoke();

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

                builder.AddElementReferenceCapture(++seq, r => _elementRef = r);

                ++seq;
                if (width != null) builder.AddAttribute(seq, "style", $"max-width: {width}px");

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
                    string? text = _spec.PlaceholderText?.Invoke();
                    text ??= _type.HasFlag(Type.Single)
                        ? "Drag and drop a file here,<br>or click to select one from your computer"
                        : "Drag and drop files here,<br>or click to select from your computer";

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-NoSelectionsText");
                    builder.AddMarkupContent(++seq, text);

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
            });
    }
}