using System.Collections.Generic;
using Bogus;
using Integrant4.Element.Constructs.Tags;
using Microsoft.AspNetCore.Components;

namespace Web.Pages
{
    [Route("/constructs")]
    public partial class Constructs
    {
        private class User
        {
            public int    ID        { get; }
            public string FirstName { get; }
            public string LastName  { get; }

            public List<ITag> Tags { get; }
        }

        private TagSelector _tagSelector;

        protected override void OnInitialized()
        {
            Faker<User> b = new Faker<User>()
               .RuleFor(o => o.ID, f => f.IndexFaker)
               .RuleFor(o => o.FirstName, f => f.Name.FirstName())
               .RuleFor(o => o.LastName, f => f.Name.LastName());

            List<User> users = b.Generate(100);

            Faker<IntTag> shiftTagFaker = new Faker<IntTag>()
               .RuleFor(o => o.Name, _ => "Shift")
               .RuleFor(o => o.Value, f => f.PickRandom(1, 2, 3));

            Faker<BoolTag> fullTimeTagFaker = new Faker<BoolTag>()
               .RuleFor(o => o.Name, _ => "Is full time")
               .RuleFor(o => o.Value, v => v.PickRandom(true, false));

            Faker<StringTag> birthdayMonthTagFaker = new Faker<StringTag>()
               .RuleFor(o => o.Name, _ => "Birthday month")
               .RuleFor(o => o.Value, f => f.Date.Month());
            
            

            _tagSelector = new TagSelector(new HashSet<(TagType, string)>
            {
                (TagType.String, "Work area"),
                (TagType.Bool, "Is contractor"),
                (TagType.Bool, "Has gone through training"),
                (TagType.Bool, "Is part time"),
                (TagType.Int, "Shift"),
            }, true);

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