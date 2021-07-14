using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.Tagging;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class TagSelectorInput : IWritableInput<IReadOnlyList<ITag>>
    {
        private readonly TagSelector _tagSelector;

        public TagSelectorInput(TagSelector tagSelector)
        {
            _tagSelector = tagSelector;

            _tagSelector.OnChange += v => OnChange?.Invoke(v);
        }

        public Task SetValue(IReadOnlyList<ITag>? value, bool invokeOnChange = true)
        {
            _tagSelector.SetTags(value ?? Array.Empty<ITag>());
            return Task.CompletedTask;
        }

        public RenderFragment Renderer() => _tagSelector.Renderer();

        public IReadOnlyList<ITag>? GetValue()
        {
            IReadOnlyList<ITag> tags = _tagSelector.GetValue();
            return tags.Count == 0 ? null : tags;
        }

        public Task<IReadOnlyList<ITag>?> ReadValue() => Task.FromResult(GetValue());

        public event Action<IReadOnlyList<ITag>?>? OnChange;
    }
}