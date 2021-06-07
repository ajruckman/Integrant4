using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Superset.Web.Markup;

namespace Integrant4.Element.Constructs.TagSelector
{
    public partial class TagSelector : IConstruct
    {
        private readonly HashSet<(TagType, string)> _knownTags;

        private List<ITag> _tags;

        public TagSelector(HashSet<(TagType, string)> knownTags)
        {
            _knownTags = knownTags;

            _tags = new List<ITag>();

            // _validationState = new ValidationState()

            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort)(12));

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
        private readonly Debouncer<string?> _tagNameDebouncer;
        private readonly Debouncer<object?> _tagValueDebouncer;

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

        private readonly BootstrapIcon _deselectValueButton;

        public RenderFragment Renderer() => Latch.Create
        (
            builder =>
            {
                int seq = -1;

                ServiceInjector<IJSRuntime>.Inject(builder, ref seq, v => _jsRuntime          = v);
                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector");

                builder.OpenElement(++seq, "section");

                // Tags

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Tags");

                for (var i = 0; i < _tags.Count; i++)
                {
                    int  i1  = i;
                    ITag tag = _tags[i];

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Tag");

                    builder.OpenElement(++seq, "div");
                    builder.AddContent(++seq, tag.Name);
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

                // Inputs

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Inputs");

                // builder.OpenElement(++seq, "select");
                // builder.AddAttribute(++seq, "value", _newTagType.ToString());
                // builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, SetNewTagType));
                // builder.AddElementReferenceCapture(++seq, r => _newTagTypeInputRef = r);
                // builder.OpenElement(++seq, "option");
                // builder.AddAttribute(++seq, "value", "String");
                // builder.AddContent(++seq, "Text");
                // builder.CloseElement();
                // builder.OpenElement(++seq, "option");
                // builder.AddAttribute(++seq, "value", "Int");
                // builder.AddContent(++seq, "Number");
                // builder.CloseElement();
                // builder.OpenElement(++seq, "option");
                // builder.AddAttribute(++seq, "value", "Bool");
                // builder.AddContent(++seq, "Boolean");
                // builder.CloseElement();
                // builder.CloseElement();

                // builder.OpenElement(++seq, "input");
                // builder.AddAttribute(++seq, "type", "text");
                // builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, _tagNameDebouncer.Reset));
                // builder.AddElementReferenceCapture(++seq, r => _newTagNameInputRef = r);
                // builder.CloseElement();
                //
                // builder.OpenElement(++seq, "input");
                // builder.AddAttribute(++seq, "type", _newTagType switch
                // {
                //     TagType.String => "text",
                //     TagType.Int => "number",
                //     TagType.Bool => "checkbox",
                //     _ => throw new ArgumentOutOfRangeException(nameof(_newTagType), "Unmatched new tag type."),
                // });
                // builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, _tagValueDebouncer.Reset));
                // builder.CloseElement();

                builder.AddContent(++seq, _newTagTypeSelector?.Renderer());
                builder.AddContent(++seq, _newTagNameInput?.Renderer());

                //

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

                //

                builder.OpenElement(++seq, "button");
                builder.AddAttribute(++seq, "disabled", !CanAddTag());
                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, AddTag));
                builder.AddContent(++seq, "+");
                builder.CloseElement();

                builder.CloseElement();

                //

                builder.CloseElement();

                // Known tags

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-KnownTags");

                builder.OpenElement(++seq, "h3");
                builder.AddContent(++seq, "Known tags");
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
                    builder.AddContent(++seq, tagType);
                    builder.CloseElement();

                    builder.CloseElement();
                    builder.CloseElement();
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
                    new Option<TagType>(TagType.Bool, "Boolean"),
                }, (l, r) => l == r);
            _newTagTypeSelector.OnChange += async v => await SetNewTagType(v);

            _newTagNameInput          =  new TextInput(jsRuntime, _newTagName);
            _newTagNameInput.OnChange += _tagNameDebouncer.Reset;

            _newTagStringInput          =  new TextInput(jsRuntime, null);
            _newTagStringInput.OnChange += v => _tagValueDebouncer.Reset(v);

            _newTagIntegerInput          =  new IntegerInput(jsRuntime, null);
            _newTagIntegerInput.OnChange += v => _tagValueDebouncer.Reset(v);

            _newTagBooleanInput          =  new CheckboxInput(jsRuntime, false);
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

        private bool CanAddTag() => !_busy && (_newTagName != null && _newTagValue != null);

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