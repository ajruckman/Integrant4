using System.Collections.Generic;
using Integrant4.Element.Constructs.TagSelector;
using Microsoft.AspNetCore.Components;

namespace Web.Pages
{
    [Route("/constructs")]
    public partial class Constructs
    {
        private TagSelector _tagSelector;

        protected override void OnInitialized()
        {
            _tagSelector = new TagSelector(new HashSet<(TagType, string)>
            {
                (TagType.String, "Work area"),
                (TagType.Bool, "Is contractor"),
                (TagType.Bool, "Has gone through training"),
                (TagType.Bool, "Is part time"),
                (TagType.Int, "Shift"),
            });

            _tagSelector.AddTag(new StringTag("Name", "John"));
            _tagSelector.AddTag(new IntTag("Shift", 1));
            _tagSelector.AddTag(new BoolTag("Is intern", true));
            _tagSelector.AddTag(new StringTag("Birthday month", "May"));
            _tagSelector.AddTag(new IntTag("Age", 25));
            _tagSelector.AddTag(new BoolTag("Has pets", false));
            _tagSelector.AddTag(new BoolTag("Is married", false));
        }
    }
}