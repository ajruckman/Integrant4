using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.Tags;
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

        public Task<IReadOnlyList<ITag>?> GetValue() =>
            Task.FromResult(_tagSelector.Tags.Count == 0 ? null : _tagSelector.Tags);

        public event Action<IReadOnlyList<ITag>?>? OnChange;
    }
}