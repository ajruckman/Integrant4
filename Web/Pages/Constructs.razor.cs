using System.Collections.Generic;
using System.Linq;
using Bogus;
using Integrant4.API;
using Integrant4.Element;
using Integrant4.Element.Constructs;
using Integrant4.Element.Constructs.Selectors;
using Integrant4.Element.Constructs.Tagging;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    [Route("/constructs")]
    public partial class Constructs
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        private class User
        {
            public User(int id, string firstName, string lastName, List<ITag> tags)
            {
                ID        = id;
                FirstName = firstName;
                LastName  = lastName;
                Tags      = tags;
            }

            public int    ID        { get; }
            public string FirstName { get; }
            public string LastName  { get; }

            public List<ITag> Tags { get; }
        }

        private TagSelector          _tagSelector = null!;
        private TagSelector          _basicTagSelector = null!;
        private List<User>           _users       = null!;
        private IReadOnlyList<ITag>? _filters;
        private Selector<User>       _selector = null!;

        protected override void OnInitialized()
        {
            Faker<IntTag> shiftTagFaker = new Faker<IntTag>()
               .CustomInstantiator(f => new IntTag("Shift", f.PickRandom(1, 2, 3)));

            Faker<BoolTag> fullTimeTagFaker = new Faker<BoolTag>()
               .CustomInstantiator(f => new BoolTag("Is full time", f.Random.Bool()));

            Faker<StringTag> birthdayMonthTagFaker = new Faker<StringTag>()
               .CustomInstantiator(f => new StringTag("Birthday month", f.Date.Month()));

            Faker<IntTag> startYearFaker = new Faker<IntTag>()
               .CustomInstantiator(f => new IntTag("Start year", f.Random.Long(1920, 2021)));

            Faker<User> b = new Faker<User>().CustomInstantiator(f =>
            {
                User result = new
                (
                    f.IndexFaker,
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    new List<ITag>()
                );

                result.Tags.Add(shiftTagFaker.Generate());
                result.Tags.Add(fullTimeTagFaker.Generate());
                result.Tags.Add(birthdayMonthTagFaker.Generate());

                if (f.Random.Bool()) result.Tags.Add(startYearFaker.Generate());

                return result;
            });

            _users = b.Generate(100);

            //

            ITag[] fullTimeTagFilter        = { new BoolTag("Is full time", true) };
            ITag[] partTimeThirdShiftFilter = { new BoolTag("Is full time", true), new IntTag("Shift", 3) };

            // List<User> fullTimes = 
            // users.Where(v => TagMatcher.Matches(v.Tags, fullTimeTagFilter)).ToList();
            // List<User> partTimeThirdShifts =
            // users.Where(v => TagMatcher.Matches(v.Tags, partTimeThirdShiftFilter)).ToList();

            // Console.WriteLine(fullTimes.Count);
            // Console.WriteLine(partTimeThirdShifts.Count);

            //

            _tagSelector = new TagSelector(new HashSet<(TagType, string)>
            {
                (TagType.String, "Work area"),
                (TagType.Bool, "Is contractor"),
                (TagType.Bool, "Has gone through training"),
                (TagType.Int, "Shift"),
                (TagType.Bool, "Is full time"),
                (TagType.String, "Birthday month"),
                (TagType.Int, "Start year"),
            }, true, spec: new TagSelector.Spec
            {
                DefaultFilterByNameOnly = false,
            });

            _basicTagSelector = new TagSelector(new HashSet<(TagType, string)>
            {
                (TagType.String, "Work area"),
                (TagType.Bool, "Is contractor"),
                (TagType.Bool, "Has gone through training"),
                (TagType.Int, "Shift"),
                (TagType.Bool, "Is full time"),
                (TagType.String, "Birthday month"),
                (TagType.Int, "Start year"),
            }, true, spec: new TagSelector.Spec
            {
                BasicMode = true,
                DefaultFilterByNameOnly = false,
            });

            // _tagSelector.AddTag(new StringTag("Name", "John"));
            // _tagSelector.AddTag(new IntTag("Shift", 1));
            // _tagSelector.AddTag(new BoolTag("Is intern", true));
            // _tagSelector.AddTag(new StringTag("Birthday month", "May"));
            // _tagSelector.AddTag(new IntTag("Age", 25));
            // _tagSelector.AddTag(new BoolTag("Has pets", false));
            // _tagSelector.AddTag(new BoolTag("Is married", false));

            _tagSelector.OnChange += filters =>
            {
                _filters = filters;
                InvokeAsync(StateHasChanged);
            };

            //

            _selector = new Selector<User>(JSRuntime, () => _users.Select(v => new Selector<User>.Option
            (
                v,
                new FlexColumn(ContentRef.Static(new IRenderable[]
                {
                    v.FirstName.AsTextContent(weight: FontWeight.Bold),
                    v.ID.ToString().AsContent(),
                })).Renderer(),
                v.FirstName.AsContent(),
                v.FirstName,
                false, false
            )).ToArray());
        }

        private List<User> MatchedUsers() =>
            _filters == null || _filters.Count == 0
                ? _users
                : _users.Where(v => TagMatcher.Matches(v.Tags, _filters)).ToList();
    }
}