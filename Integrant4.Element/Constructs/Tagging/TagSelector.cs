using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.Element.Bits;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs.Tagging
{
    public partial class TagSelector : IConstruct
    {
        private readonly HashSet<(TagType, string)> _knownTags;
        private readonly bool                       _isForFiltering;
        private readonly Spec                       _spec;
        private readonly List<ITag>                 _tags     = new();
        private readonly List<(TagType, string)>    _tagsUsed = new();

        public TagSelector
        (
            HashSet<(TagType, string)> knownTags,
            bool                       isForFiltering,
            Spec?                      spec = null
        )
        {
            _knownTags      = knownTags;
            _isForFiltering = isForFiltering;
            _spec           = spec ?? new Spec();
            _acceptAnyValue = isForFiltering ? _spec.DefaultFilterByNameOnly : null;

            if (_spec.Value != null)
            {
                SetTags(_spec.Value.Invoke(), false);
            }

            _addButton = new Button("Add".AsStatic(), new Button.Spec
            {
                Style      = () => Button.Style.Green,
                IsDisabled = () => !CanAddTag(),
                OnClick    = async (_, _) => await AddTag(),
                Scale      = _spec.Scale,
                Height     = () => 27,
            });
            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort) (12 * _spec.Scale?.Invoke() ?? 12));

            _tagNameDebouncer = new Debouncer<string?>(() =>
            {
                _busy = true;
                _refresher?.Invoke();
            }, value =>
            {
                _newTagName = value;

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

                _busy = false;
                _refresher?.Invoke();
            }, 250);

            //

            _tagsListHeaderText = !isForFiltering
                ? "Selected tags"
                : "Tag filters";

            _tagsAddHeaderText = !isForFiltering
                ? "Add tag"
                : "Add tag filter";

            _tagsPreviousHeaderText = !isForFiltering
                ? "Apply previous tag"
                : "Tags in use";
        }

        public IReadOnlyList<ITag> GetValue() => _tags;

        public event Action<IReadOnlyList<ITag>?>? OnChange;
    }

    public partial class TagSelector
    {
        public class Spec
        {
            public bool DefaultFilterByNameOnly { get; init; } = true;
            public bool BasicMode               { get; init; } = false;

            public Callbacks.Callback<IReadOnlyList<ITag>>? Value             { get; init; }
            public Callbacks.IsDisabled?                    IsDisabled        { get; init; }
            public Callbacks.Unit?                          MaxTotalWidth     { get; init; }
            public Callbacks.Unit?                          SelectedTagsWidth { get; init; }
            public Callbacks.Unit?                          KnownTagsWidth    { get; init; }
            public Callbacks.Scale?                         Scale             { get; init; }
        }
    }

    public partial class TagSelector
    {
        private readonly Debouncer<string?> _tagNameDebouncer;
        private readonly Debouncer<object?> _tagValueDebouncer;

        private readonly BootstrapIcon _deselectValueButton;
        private readonly Button        _addButton;

        private readonly string _tagsListHeaderText;
        private readonly string _tagsAddHeaderText;
        private readonly string _tagsPreviousHeaderText;

        private IJSRuntime?     _jsRuntime;
        private ElementService? _elementService;
        private WriteOnlyHook?  _refresher;

        private TagType _newTagType;
        private string? _newTagName;
        private object? _newTagValue;
        private bool?   _acceptAnyValue;

        private bool _busy;

        private bool                  _hasInitInputs;
        private SelectInput<TagType>? _newTagTypeSelector;
        private TextInput?            _newTagNameInput;
        private TextInput?            _newTagStringInput;
        private LongInput?            _newTagIntegerInput;
        private CheckboxInput?        _newTagBooleanInput;
        private CheckboxInput?        _anyValueInput;

        public RenderFragment Renderer() => Latch.Create
        (
            builder =>
            {
                int seq = -1;

                ServiceInjector<IJSRuntime>.Inject(builder, ref seq, v => _jsRuntime          = v);
                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector");
                builder.AddAttribute(++seq, "style",
                    $"max-width: {_spec.MaxTotalWidth?.Invoke().Serialize() ?? "600px"}");

                // Left

                // builder.OpenElement(++seq, "section");
                // builder.AddAttribute(++seq, "style",
                // $"width: {_spec.MaxTotalWidth?.Invoke().Serialize() ?? "300px"}");

                {
                    // Tags

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-Tags");
                    builder.AddAttribute(++seq, "style",
                        $"width: {_spec.SelectedTagsWidth?.Invoke().Serialize() ?? "unset"}");

                    builder.OpenElement(++seq, "h3");
                    builder.AddContent(++seq, _tagsListHeaderText);
                    builder.CloseElement();

                    for (var i = 0; i < _tags.Count; i++)
                    {
                        int  i1  = i;
                        ITag tag = _tags[i];

                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-RemovableTag");

                        builder.OpenElement(++seq, "div");
                        builder.AddContent(++seq, $"{tag.Name}" + (_spec.BasicMode ? "" : ":"));
                        builder.CloseElement();

                        if (!_spec.BasicMode)
                        {
                            builder.OpenElement(++seq, "div");
                            builder.AddContent(++seq, tag.Content());
                            builder.CloseElement();
                        }

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
                    // Known tags

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-KnownTags");
                    builder.AddAttribute(++seq, "style",
                        $"width: {_spec.KnownTagsWidth?.Invoke().Serialize() ?? "unset"}");

                    builder.OpenElement(++seq, "h3");
                    builder.AddContent(++seq, _tagsPreviousHeaderText);
                    builder.CloseElement();

                    foreach ((TagType tagType, string tagName) in _knownTags)
                    {
                        builder.OpenElement(++seq, "span");
                        builder.OpenElement(++seq, "button");

                        ++seq;
                        if (_newTagType == tagType && _newTagName == tagName)
                            builder.AddAttribute(seq, "data-current");

                        bool used = _tagsUsed.Contains((tagType, tagName));
                        builder.AddAttribute(++seq, "disabled", used);

                        builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this,
                            async () => await UseKnownTag(tagType, tagName)));

                        builder.AddContent(++seq, tagName);

                        if (!_spec.BasicMode)
                        {
                            builder.OpenElement(++seq, "span");
                            builder.AddContent(++seq, tagType switch
                            {
                                TagType.String => "Text",
                                TagType.Int    => "Number",
                                TagType.Bool   => "Truthy",
                                _              => throw new ArgumentOutOfRangeException(),
                            });
                            builder.CloseElement();
                        }

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
                    builder.AddContent(++seq, _tagsAddHeaderText);
                    builder.CloseElement();

                    {
                        builder.OpenElement(++seq, "table");
                        {
                            if (!_spec.BasicMode)
                            {
                                {
                                    // Type
                                    builder.OpenElement(++seq, "tr");
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

                                if (_isForFiltering)
                                {
                                    // Any value checkbox
                                    builder.OpenElement(++seq, "tr");
                                    builder.OpenElement(++seq, "td");
                                    builder.AddContent(++seq, "Match any value");
                                    builder.CloseElement();
                                    builder.OpenElement(++seq, "td");

                                    builder.OpenElement(++seq, "div");
                                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-RowContainer");
                                    {
                                        builder.OpenElement(++seq, "div");
                                        builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-ValueInput");
                                        builder.AddContent(++seq, _anyValueInput?.Renderer());
                                        builder.CloseElement();

                                        if (_acceptAnyValue == true)
                                        {
                                            builder.OpenElement(++seq, "div");
                                            builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-AddButton");
                                            builder.AddContent(++seq, _addButton.Renderer());
                                            builder.CloseElement();
                                        }
                                    }
                                    builder.CloseElement();
                                    builder.CloseElement();
                                    builder.CloseElement();
                                }

                                {
                                    // Value
                                    if (!_isForFiltering || _isForFiltering && _acceptAnyValue == false)
                                    {
                                        builder.OpenElement(++seq, "tr");
                                        builder.OpenElement(++seq, "td");
                                        builder.AddContent(++seq, "Value");
                                        builder.CloseElement();
                                        builder.OpenElement(++seq, "td");

                                        builder.OpenElement(++seq, "div");
                                        builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-RowContainer");
                                        {
                                            builder.OpenElement(++seq, "div");
                                            builder.AddAttribute(++seq, "class",
                                                "I4E-Construct-TagSelector-ValueInput");
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

                                            builder.OpenElement(++seq, "div");
                                            builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-AddButton");
                                            builder.AddContent(++seq, _addButton.Renderer());
                                            builder.CloseElement();
                                        }
                                        builder.CloseElement();

                                        builder.CloseElement();
                                        builder.CloseElement();
                                    }
                                }
                            }
                            else
                            {
                                builder.OpenElement(++seq, "tr");
                                builder.OpenElement(++seq, "td");
                                builder.AddContent(++seq, "Value");
                                builder.CloseElement();
                                builder.OpenElement(++seq, "td");
                                builder.OpenElement(++seq, "div");
                                builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-RowContainer");
                                {
                                    builder.OpenElement(++seq, "div");
                                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-ValueInput");
                                    builder.AddContent(++seq, _newTagNameInput?.Renderer());
                                    builder.CloseElement();

                                    builder.OpenElement(++seq, "div");
                                    builder.AddAttribute(++seq, "class", "I4E-Construct-TagSelector-AddButton");
                                    builder.AddContent(++seq, _addButton.Renderer());
                                    builder.CloseElement();
                                }
                                builder.CloseElement();
                                builder.CloseElement();
                                builder.CloseElement();
                            }
                        }

                        // {
                        //     // Add button
                        //     builder.OpenElement(++seq, "tr");
                        //     builder.OpenElement(++seq, "td");
                        //     builder.AddAttribute(++seq, "colspan", 2);
                        //     builder.CloseElement();
                        //     builder.CloseElement();
                        // }
                        // Close table
                        builder.CloseElement();
                    }

                    builder.CloseElement();
                }

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
            _newTagTypeSelector = new SelectInput<TagType>(jsRuntime, _newTagType, () =>
                new SelectInput<TagType>.IOption[]
                {
                    new SelectInput<TagType>.Option(TagType.String, "String", true),
                    new SelectInput<TagType>.Option(TagType.Int,    "Number"),
                    new SelectInput<TagType>.Option(TagType.Bool,   "Truthy"),
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

            _newTagIntegerInput = new LongInput(jsRuntime, null, new LongInput.Spec
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

            //

            if (_isForFiltering)
            {
                _anyValueInput = new CheckboxInput(jsRuntime, _spec.DefaultFilterByNameOnly, new CheckboxInput.Spec
                {
                    IsDisabled = _spec.IsDisabled,
                });

                _anyValueInput.OnChange += v =>
                {
                    _acceptAnyValue = v;
                    _refresher?.Invoke();
                };
            }

            //

            _hasInitInputs = true;
        }

        private void RefreshUsedTags()
        {
            _tagsUsed.Clear();
            _tagsUsed.AddRange(_tags.Select(MapTag));
        }

        private void RemoveTag(int i)
        {
            _tags.RemoveAt(i);
            RefreshUsedTags();
            OnChange?.Invoke(_tags.Count == 0 ? null : _tags);
            _refresher?.Invoke();
        }

        private async Task SetNewTagType(TagType tagType)
        {
            _newTagType = tagType;

            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await ClearAllValueInputs();

            _refresher?.Invoke();
        }

        private bool CanAddTag() =>
            !_busy                                                                                          &&
            _newTagName != null                                                                             &&
            (_newTagValue     != null || ((_isForFiltering && _acceptAnyValue == true) || _spec.BasicMode)) &&
            (_spec.IsDisabled == null || _spec.IsDisabled.Invoke() == false);

        private async Task AddTag()
        {
            if (_newTagName == null) return;

            if ((!_isForFiltering || _acceptAnyValue == false) && !_spec.BasicMode)
            {
                if (_newTagValue == null) return;

                _tags.Add(_newTagType switch
                {
                    TagType.String => new StringTag(_newTagName, (string) _newTagValue),
                    TagType.Int    => new IntTag(_newTagName, (long) _newTagValue),
                    TagType.Bool   => new BoolTag(_newTagName, (bool) _newTagValue),
                    _              => throw new ArgumentOutOfRangeException(),
                });
            }
            else if (_acceptAnyValue == true)
            {
                _tags.Add(_newTagType switch
                {
                    TagType.String => new AnyStringTag(_newTagName),
                    TagType.Int    => new AnyIntTag(_newTagName),
                    TagType.Bool   => new AnyBoolTag(_newTagName),
                    _              => throw new ArgumentOutOfRangeException(),
                });
            }
            else if (_spec.BasicMode)
            {
                if (_tags.Any(v => v.Name == _newTagName))
                    return;
                _tags.Add(new AnyStringTag(_newTagName));
            }

            _knownTags.Add((_newTagType, _newTagName));
            RefreshUsedTags();

            _newTagName  = null;
            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await ClearAllValueInputs();

            OnChange?.Invoke(_tags.Count == 0 ? null : _tags);
            _refresher?.Invoke();
        }

        private async Task ClearAllValueInputs()
        {
            await (_newTagNameInput?.SetValue(null, false) ?? Task.CompletedTask);

            if (_acceptAnyValue != true)
            {
                await (_newTagStringInput?.SetValue(null, false)   ?? Task.CompletedTask);
                await (_newTagIntegerInput?.SetValue(null, false)  ?? Task.CompletedTask);
                await (_newTagBooleanInput?.SetValue(false, false) ?? Task.CompletedTask);
            }
        }

        public void AddTag(ITag tag, bool invokeOnChange = true)
        {
            _tags.Add(tag);

            _knownTags.Add(MapTag(tag));
            RefreshUsedTags();

            if (invokeOnChange) OnChange?.Invoke(_tags.Count == 0 ? null : _tags);
            _refresher?.Invoke();
        }

        public void SetTags(IReadOnlyList<ITag> tags, bool invokeOnChange = true)
        {
            _tags.Clear();
            _tags.AddRange(tags);

            foreach (ITag tag in tags)
            {
                _knownTags.Add(MapTag(tag));
            }

            RefreshUsedTags();

            if (invokeOnChange) OnChange?.Invoke(_tags.Count == 0 ? null : _tags);
            _refresher?.Invoke();
        }

        private (TagType, string Name) MapTag(ITag tag) => tag switch
        {
            StringTag    => (TagType.String, tag.Name),
            IntTag       => (TagType.Int, tag.Name),
            BoolTag      => (TagType.Bool, tag.Name),
            AnyStringTag => (TagType.String, tag.Name),
            AnyIntTag    => (TagType.Int, tag.Name),
            AnyBoolTag   => (TagType.Bool, tag.Name),
            _            => throw new ArgumentOutOfRangeException(nameof(tag), tag, null),
        };

        private async Task UseKnownTag(TagType type, string name)
        {
            if (_newTagType != type) _newTagValue = null;

            _newTagType  = type;
            _newTagName  = name;
            _newTagValue = _newTagType == TagType.Bool ? false : null;

            await (_newTagTypeSelector?.SetValue(type)     ?? Task.CompletedTask);
            await (_newTagNameInput?.SetValue(name, false) ?? Task.CompletedTask);

            if (_acceptAnyValue != true)
            {
                await (_newTagStringInput?.SetValue(null, false)   ?? Task.CompletedTask);
                await (_newTagIntegerInput?.SetValue(null, false)  ?? Task.CompletedTask);
                await (_newTagBooleanInput?.SetValue(false, false) ?? Task.CompletedTask);
            }

            _refresher?.Invoke();
        }
    }
}