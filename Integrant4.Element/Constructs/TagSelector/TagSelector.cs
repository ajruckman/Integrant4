using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.Element.Bits;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs.TagSelector
{
    public partial class TagSelector : IConstruct
    {
        private readonly HashSet<(TagType, string)> _knownTags;
        private readonly Spec                       _spec;

        private List<ITag> _tags = new();

        public TagSelector(HashSet<(TagType, string)> knownTags, Spec? spec = null)
        {
            _knownTags = knownTags;
            _spec      = spec ?? new Spec();

            _addButton = new Button(() => "Add".AsContent(), new Button.Spec
            {
                Style      = () => Button.Style.Green,
                IsDisabled = () => !CanAddTag(),
                OnClick    = async (_, _) => await AddTag(),
                Scale      = _spec.Scale,
            });
            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort)(12 * _spec.Scale?.Invoke() ?? 12));

            _tagNameDebouncer = new Debouncer<string?>(() =>
            {
                _busy = true;
                _refresher?.Invoke();
            }, value =>
            {
                _newTagName = value;
                Console.WriteLine($"Name -> {_newTagName}");

                _busy = false;
                _refresher?.Invoke();
            }, 250);

            _tagValueDebouncer = new Debouncer<object?>(() =>
            {
                _busy = true;
                _refresher?.Invoke();
            }, value =>
            {
                // _newTagValue = args.Value?.ToString().CoalesceAndTrim();
                _newTagValue = value;
                Console.WriteLine($"Value -> {_newTagValue}");

                _busy = false;
                _refresher?.Invoke();
            }, 250);

            //
        }

        public IReadOnlyList<ITag> Tags => _tags;
    }

    public partial class TagSelector
    {
        public class Spec
        {
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.Pixels?     LeftWidth  { get; init; }
            public Callbacks.Pixels?     RightWidth { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }
        }
    }

    public partial class TagSelector
    {
        private readonly Debouncer<string?> _tagNameDebouncer;
        private readonly Debouncer<object?> _tagValueDebouncer;

        private readonly BootstrapIcon _deselectValueButton;

        private IJSRuntime?     _jsRuntime;
        private ElementService? _elementService;
        private WriteOnlyHook?  _refresher;

        private TagType _newTagType;
        private string? _newTagName;
        private object? _newTagValue;

        private bool _busy;

        private bool                  _hasInitInputs;
        private SelectInput<TagType>? _newTagTypeSelector;
        private TextInput?            _newTagNameInput;
        private TextInput?            _newTagStringInput;
        private IntegerInput?         _newTagIntegerInput;
        private CheckboxInput?        _newTagBooleanInput;
        private Button                _addButton;

        public RenderFragment Renderer() => Latch.Create
        (
            builder =>
            {
                int seq = -1;

                ServiceInjector<IJSRuntime>.Inject(builder, ref seq, v => _jsRuntime          = v);
                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector");

                // Left

                builder.OpenElement(++seq, "section");

                {
                    // Tags

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Tags");
                    builder.AddAttribute(++seq, "style", $"width: {_spec?.LeftWidth?.Invoke() ?? 300}px");

                    builder.OpenElement(++seq, "h3");
                    builder.AddContent(++seq, "Tags");
                    builder.CloseElement();

                    for (var i = 0; i < _tags.Count; i++)
                    {
                        int  i1  = i;
                        ITag tag = _tags[i];

                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Tag");

                        builder.OpenElement(++seq, "div");
                        builder.AddContent(++seq, $"{tag.Name}:");
                        builder.CloseElement();

                        builder.OpenElement(++seq, "div");
                        builder.AddContent(++seq, tag.Content().Renderer());
                        builder.CloseElement();

                        // builder.OpenElement(++seq, "button");
                        // builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, () => RemoveTag(i1)));
                        // builder.AddContent(++seq, "x");
                        // builder.CloseElement();

                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector-DeselectButtonWrapper");
                        builder.AddAttribute(++seq, "tabindex", 0);
                        builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, () => RemoveTag(i1)));
                        builder.AddContent(++seq, _deselectValueButton.Renderer());
                        builder.CloseElement();

                        builder.CloseElement();
                    }

                    builder.CloseElement();
                }

                {
                    // Inputs

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Inputs");

                    builder.OpenElement(++seq, "h3");
                    builder.AddContent(++seq, "Add tag");
                    builder.CloseElement();

                    {
                        builder.OpenElement(++seq, "table");
                        builder.OpenElement(++seq, "tr");
                        {
                            // Type
                            builder.OpenElement(++seq, "td");
                            builder.AddContent(++seq, "Type");
                            builder.CloseElement();
                            builder.OpenElement(++seq, "td");
                            builder.AddContent(++seq, _newTagTypeSelector?.Renderer());
                            builder.CloseElement();
                            builder.CloseElement();
                        }
                        {
                            // Name
                            builder.OpenElement(++seq, "tr");
                            builder.OpenElement(++seq, "td");
                            builder.AddContent(++seq, "Name");
                            builder.CloseElement();
                            builder.OpenElement(++seq, "td");
                            builder.AddContent(++seq, _newTagNameInput?.Renderer());
                            builder.CloseElement();
                            builder.CloseElement();
                        }
                        {
                            // Value
                            builder.OpenElement(++seq, "tr");
                            builder.OpenElement(++seq, "td");
                            builder.AddContent(++seq, "Value");
                            builder.CloseElement();
                            builder.OpenElement(++seq, "td");
                            builder.OpenElement(++seq, "span");
                            builder.AddAttribute(++seq, "hidden", _newTagType != TagType.String);
                            builder.AddContent(0, _newTagStringInput?.Renderer());
                            builder.CloseElement();
                            builder.OpenElement(++seq, "span");
                            builder.AddAttribute(++seq, "hidden", _newTagType != TagType.Int);
                            builder.AddContent(1, _newTagIntegerInput?.Renderer());
                            builder.CloseElement();
                            builder.OpenElement(++seq, "span");
                            builder.AddAttribute(++seq, "hidden", _newTagType != TagType.Bool);
                            builder.AddContent(2, _newTagBooleanInput?.Renderer());
                            builder.CloseElement();
                            builder.CloseElement();
                            builder.CloseElement();
                        }
                        {
                            // Add button
                            builder.OpenElement(++seq, "tr");
                            builder.OpenElement(++seq, "td");
                            builder.AddAttribute(++seq, "colspan", 2);
                            builder.AddContent(++seq, _addButton.Renderer());
                            builder.CloseElement();
                            builder.CloseElement();
                        }
                        // Close table
                        builder.CloseElement();
                    }

                    builder.CloseElement();
                }

                builder.CloseElement();

                // Right

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-KnownTags");
                builder.AddAttribute(++seq, "style", $"width: {_spec?.RightWidth?.Invoke() ?? 300}px");

                {
                    // Known tags

                    builder.OpenElement(++seq, "h3");
                    builder.AddContent(++seq, "Apply previous tag");
                    builder.CloseElement();

                    foreach ((TagType tagType, string tagName) in _knownTags)
                    {
                        builder.OpenElement(++seq, "span");
                        builder.OpenElement(++seq, "button");
                        ++seq;
                        if (_newTagType == tagType && _newTagName == tagName)
                            builder.AddAttribute(seq, "data-current");
                        builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this,
                            async () => await UseKnownTag(tagType, tagName)));

                        builder.AddContent(++seq, tagName);
                        builder.OpenElement(++seq, "span");
                        builder.AddContent(++seq, tagType switch
                        {
                            TagType.String => "Text",
                            TagType.Int    => "Number",
                            TagType.Bool   => "Truthy",
                            _              => throw new ArgumentOutOfRangeException(),
                        });
                        builder.CloseElement();

                        builder.CloseElement();
                        builder.CloseElement();
                    }
                }

                builder.CloseElement();

                //

                builder.CloseElement();
            },
            v => _refresher = v,
            firstRender =>
            {
                if (_jsRuntime == null || _elementService == null)
                    throw new ArgumentException("Missing one or more required services.");

                if (!_hasInitInputs && _jsRuntime != null)
                {
                    InitInputs(_jsRuntime);
                    _refresher!.Invoke();
                }

                return Task.CompletedTask;
            }
        );

        private void InitInputs(IJSRuntime jsRuntime)
        {
            _newTagTypeSelector = new SelectInput<TagType>(jsRuntime, TagType.String, () =>
                new IOption<TagType>[]
                {
                    new Option<TagType>(TagType.String, "String", true),
                    new Option<TagType>(TagType.Int, "Number"),
                    new Option<TagType>(TagType.Bool, "Truthy"),
                }, (l, r) => l == r, new SelectInput<TagType>.Spec
            {
                IsDisabled = _spec.IsDisabled,
                Scale      = _spec.Scale,
            });

            _newTagNameInput = new TextInput(jsRuntime, _newTagName, new TextInput.Spec
            {
                IsDisabled = _spec.IsDisabled,
                Scale      = _spec.Scale,
            });

            _newTagStringInput = new TextInput(jsRuntime, null, new TextInput.Spec
            {
                IsDisabled = _spec.IsDisabled,
                Scale      = _spec.Scale,
            });

            _newTagIntegerInput = new IntegerInput(jsRuntime, null, new IntegerInput.Spec
            {
                IsDisabled = _spec.IsDisabled,
                Scale      = _spec.Scale,
            });

            _newTagBooleanInput = new CheckboxInput(jsRuntime, false, new CheckboxInput.Spec
            {
                IsDisabled = _spec.IsDisabled,
            });

            _newTagTypeSelector.OnChange += async v => await SetNewTagType(v);
            _newTagNameInput.OnChange    += _tagNameDebouncer.Reset;
            _newTagStringInput.OnChange  += v => _tagValueDebouncer.Reset(v);
            _newTagIntegerInput.OnChange += v => _tagValueDebouncer.Reset(v);
            _newTagBooleanInput.OnChange += v => _tagValueDebouncer.Reset(v);

            _hasInitInputs = true;
        }

        private void RemoveTag(int i)
        {
            Console.WriteLine($"Remove: {i}");
            _tags.RemoveAt(i);
            _refresher?.Invoke();
        }

        private async Task SetNewTagType(TagType tagType)
        {
            _newTagType = tagType;

            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await ClearAllValueInputs();

            Console.WriteLine($"Type -> {_newTagType}");

            _refresher?.Invoke();
        }

        private bool CanAddTag() =>
            !_busy && _newTagName != null && _newTagValue != null &&
            (_spec.IsDisabled == null || _spec.IsDisabled.Invoke() == false);

        private async Task AddTag()
        {
            if (_newTagName == null || _newTagValue == null) return;

            _tags.Add(_newTagType switch
            {
                TagType.String => new StringTag(_newTagName, (string)_newTagValue),
                TagType.Int    => new IntTag(_newTagName, (long)_newTagValue),
                TagType.Bool   => new BoolTag(_newTagName, (bool)_newTagValue),
                _              => throw new ArgumentOutOfRangeException(),
            });

            _knownTags.Add((_newTagType, _newTagName));

            _newTagName  = null;
            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await ClearAllValueInputs();

            _refresher?.Invoke();
        }

        private async Task ClearAllValueInputs()
        {
            await (_newTagNameInput?.SetValue(null, false)     ?? Task.CompletedTask);
            await (_newTagStringInput?.SetValue(null, false)   ?? Task.CompletedTask);
            await (_newTagIntegerInput?.SetValue(null, false)  ?? Task.CompletedTask);
            await (_newTagBooleanInput?.SetValue(false, false) ?? Task.CompletedTask);
        }

        public void AddTag(ITag tag)
        {
            _tags.Add(tag);

            _knownTags.Add(tag switch
            {
                StringTag => (TagType.String, tag.Name),
                IntTag    => (TagType.Int, tag.Name),
                BoolTag   => (TagType.Bool, tag.Name),
                _         => throw new ArgumentOutOfRangeException(nameof(tag), tag, null),
            });

            _refresher?.Invoke();
        }

        private async Task UseKnownTag(TagType tagType, string tag)
        {
            if (_newTagType != tagType) _newTagValue = null;

            _newTagType  = tagType;
            _newTagName  = tag;
            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await (_newTagTypeSelector?.SetValue(tagType)      ?? Task.CompletedTask);
            await (_newTagNameInput?.SetValue(tag, false)      ?? Task.CompletedTask);
            await (_newTagStringInput?.SetValue(null, false)   ?? Task.CompletedTask);
            await (_newTagIntegerInput?.SetValue(null, false)  ?? Task.CompletedTask);
            await (_newTagBooleanInput?.SetValue(false, false) ?? Task.CompletedTask);

            _refresher?.Invoke();
        }
    }

    public enum TagType
    {
        String, Int, Bool,
    }

    public interface ITag
    {
        string Name { get; }

        Content Content();
    }

    public interface ITag<out TValue> : ITag
    {
        TValue Value { get; }
    }

    public class StringTag : ITag<string>
    {
        public StringTag(string name, string value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public string Value { get; }

        public Content Content() => Value.AsContent();
    }

    public class IntTag : ITag<long>
    {
        public IntTag(string name, long value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public long   Value { get; }

        public Content Content() => Value.ToString().AsContent();
    }

    public class BoolTag : ITag<bool>
    {
        public BoolTag(string name, bool value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public bool   Value { get; }

        public Content Content() => (Value ? "True" : "False").AsContent();
    }
}