using Bogus;
using Integrant4.Element.Constructs;

namespace Web.Pages
{
    public partial class Tables
    {
        private PagedTable<User> _userTable = null!;
        
        protected override void OnInitialized()
        {
            Faker<User> b = new Faker<User>()
               .RuleFor(o => o.FirstName, f => f.Name.FirstName())
               .RuleFor(o => o.LastName,  f => f.Name.LastName());

            _userTable = new PagedTable<User>(() => b.Generate(460).ToArray(), 50);
        }
    }
}