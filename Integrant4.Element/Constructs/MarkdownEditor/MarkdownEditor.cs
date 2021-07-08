using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs.MarkdownEditor
{
    public partial class MarkdownEditor
    {
        private readonly DotNetObjectReference<MarkdownEditor> _objRef;
        private readonly IJSRuntime                            _jsRuntime;
        private readonly Spec?                                 _spec;

        private IJSObjectReference? _editor;
        private SpecState?          _state;

        public MarkdownEditor(IJSRuntime jsRuntime, Spec? spec = null)
        {
            _objRef    = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
            _spec      = spec;
        }

        public event Action<string?>? OnChange;

        public async Task GetValue()
        {
        }

        public async Task SetValue(string? value)
        {
        }

        public async Task Reset()
        {
            _spec?.InitialValue?.Invoke();
        }
    }

    public partial class MarkdownEditor
    {
        private ElementReference? _elemRef;
        private WriteOnlyHook?    _refresher;

        public RenderFragment Renderer() => Latch.Create
        (
            builder =>
            {
                int seq = -1;

                ClassSet classes = new("I4E-Construct", "I4E-Construct-MarkdownEditor");
                if (_spec?.IsDisabled?.Invoke() == true) classes.Add("I4E-Construct-MarkdownEditor--Disabled");

                builder.OpenElement(++seq, "section");
                builder.AddAttribute(++seq, "hidden", _spec?.IsVisible?.Invoke() == false);
                builder.AddAttribute(++seq, "class", classes.ToString());
                builder.OpenElement(++seq, "div");
                builder.AddElementReferenceCapture(++seq, r => _elemRef = r);
                builder.CloseElement();
                builder.CloseElement();
            },
            v => _refresher = v,
            async firstRender =>
            {
                Console.WriteLine(firstRender);
                if (_elemRef == null || _refresher == null)
                {
                    Console.WriteLine(
                        "Not initializing MarkdownEditor because either a required service failed to inject " +
                        "or the editor's element reference was not captured.");
                    return;
                }

                SpecState newState = new
                (
                    _spec?.PlaceholderText?.Invoke() ?? "",
                    _spec?.Height?.Invoke()          ?? 300,
                    _spec?.IsDisabled?.Invoke()      ?? false
                );

                if (firstRender)
                {
                    _editor = await _jsRuntime.InvokeAsync<IJSObjectReference>
                    (
                        "I4.Element.InitMarkdownEditor",
                        _objRef,
                        _elemRef,
                        SerializeButtons(_spec?.Buttons ?? DefaultButtons),
                        _spec?.DebounceMilliseconds   ?? 300,
                        _spec?.InitialValue?.Invoke() ?? "",
                        newState.PlaceholderText,
                        newState.Height,
                        newState.IsDisabled
                    );

                    _state = newState;
                }
                else
                {
                    if (_state?.Equals(newState) != true)
                    {
                        await _editor!.InvokeVoidAsync
                        (
                            "I4EUpdateState",
                            newState.PlaceholderText, newState.Height, newState.IsDisabled
                        );

                        _state = newState;
                    }
                }

                await Task.CompletedTask;
            }
        );
    }

    public partial class MarkdownEditor
    {
        [JSInvokable]
        public void Change(string markdown)
        {
            OnChange?.Invoke(markdown.CoalesceAndTrim());
        }
    }

    public partial class MarkdownEditor
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Button
        {
            Heading,
            Bold,
            Italic,
            Strike,
            HR,
            Quote,
            UL,
            OL,
            Task,
            Indent,
            Outdent,
            Table,
            Image,
            Link,
            Code,
            CodeBlock,
        }

        private static readonly Button[][] DefaultButtons;

        static MarkdownEditor()
        {
            DefaultButtons = new[]
            {
                new[] { Button.Heading, Button.Bold, Button.Italic },
                new[] { Button.HR, Button.Quote },
                new[] { Button.UL, Button.OL },
                new[] { Button.Image, Button.Link },
                new[] { Button.Code, Button.CodeBlock },
            };
        }

        private static string[][] SerializeButtons(Button[][] v) =>
            v.Select(x => x.Select(y => y.Serialize()).ToArray()).ToArray();
    }

    public partial class MarkdownEditor
    {
        public class Spec
        {
            public Button[][]? Buttons              { get; init; }
            public ushort      DebounceMilliseconds { get; init; }

            public Callbacks.Callback<string?>? InitialValue { get; init; }

            public Callbacks.Callback<string>? PlaceholderText { get; init; }
            public Callbacks.Pixels?           Height          { get; init; }
            public Callbacks.IsDisabled?       IsDisabled      { get; init; }
            public Callbacks.IsVisible?        IsVisible       { get; init; }
            public Callbacks.Pixels?           Width           { get; init; }
        }

        private class SpecState
        {
            public SpecState(string placeholderText, double height, bool isDisabled)
            {
                PlaceholderText = placeholderText;
                Height          = height;
                IsDisabled      = isDisabled;
            }

            public string PlaceholderText { get; }
            public double Height          { get; }
            public bool   IsDisabled      { get; }

            public bool Equals(SpecState other)
            {
                if (ReferenceEquals(this, other)) return true;
                return PlaceholderText == other.PlaceholderText &&
                       IsDisabled      == other.IsDisabled      &&
                       Height.Equals(other.Height);
            }
        }
    }

    public static class MarkdownEditorExtensions
    {
        public static string Serialize(this MarkdownEditor.Button button) => button switch
        {
            MarkdownEditor.Button.Heading   => "heading",
            MarkdownEditor.Button.Bold      => "bold",
            MarkdownEditor.Button.Italic    => "italic",
            MarkdownEditor.Button.Strike    => "strike",
            MarkdownEditor.Button.HR        => "hr",
            MarkdownEditor.Button.Quote     => "quote",
            MarkdownEditor.Button.UL        => "ul",
            MarkdownEditor.Button.OL        => "ol",
            MarkdownEditor.Button.Task      => "task",
            MarkdownEditor.Button.Indent    => "indent",
            MarkdownEditor.Button.Outdent   => "outdent",
            MarkdownEditor.Button.Table     => "table",
            MarkdownEditor.Button.Image     => "image",
            MarkdownEditor.Button.Link      => "link",
            MarkdownEditor.Button.Code      => "code",
            MarkdownEditor.Button.CodeBlock => "codeblock",
            _                               => throw new ArgumentOutOfRangeException(nameof(button), button, null),
        };
    }
}