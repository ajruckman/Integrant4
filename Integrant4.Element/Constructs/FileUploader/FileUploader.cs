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

        public FileUploader(Type type = Type.Multiple, Spec? spec = null)
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
        }

        private void OnRemove(File file)
        {
            _refresher?.Invoke();
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
    }

    public partial class FileUploader
    {
        public enum Type
        {
            Single, Multiple,
        }

        public class Spec
        {
            [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
            [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]

            public Callbacks.Callback<string>? NoSelectionsText { get; init; }

            public Callbacks.Pixels? Width { get; init; }
            public Callbacks.Scale?  Scale { get; init; }
        }
    }

    public partial class FileUploader
    {
        private readonly BootstrapIcon _deselectValueButton;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                double? width = _spec.Width?.Invoke();

                int seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService           = v);
                ServiceInjector<FileUploaderService>.Inject(builder, ref seq, v => _fileUploaderService = v);

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader");
                builder.AddElementReferenceCapture(++seq, r => _elementRef = r);

                ++seq;
                if (width != null) builder.AddAttribute(seq, "style", $"max-width: {width}px");

                // Input

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "file");
                builder.AddAttribute(++seq, "multiple", _type == Type.Multiple);
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
                        builder.SetKey(file.SerialID);
                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-File");

                        builder.OpenElement(++seq, "img");
                        builder.AddAttribute(++seq, "src", $"/api/i4/file/thumbnail/{_guid}/{file.SerialID}");
                        builder.CloseElement();

                        builder.OpenElement(++seq, "span");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-RemoveButtonWrapper");
                        builder.AddAttribute(++seq, "tabindex", 0);
                        builder.AddAttribute(++seq, "onclick",
                            EventCallback.Factory.Create(this,
                                () => _fileUploaderService?.Remove(_guid, file.SerialID)));
                        builder.AddContent(++seq, _deselectValueButton.Renderer());
                        builder.CloseElement();

                        builder.CloseElement();
                    }
                }
                else
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-FileUploader-NoSelectionsText");
                    builder.AddContent(++seq, _spec.NoSelectionsText?.Invoke() ?? "Drag and drop files here, or click to select from your computer");
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

                _fileUploaderService.Subscribe(_guid, OnAdd, OnRemove);

                await _elementService.JSInvokeVoidAsync
                (
                    "I4.Element.FileUploaderInit", _guid, _elementRef
                );
            });
    }
}