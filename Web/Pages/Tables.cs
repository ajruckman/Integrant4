using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Integrant4.Element.Constructs.Tables;
using Integrant4.Element.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Tables
    {
        private IPagedTable<User>                   _userTable                   = null!;
        private ISortablePagedTable<User>           _sortableUserTable           = null!;
        private IFilterableSortablePagedTable<User> _filterableSortableUserTable = null!;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            Faker<User> b = new Faker<User>()
               .RuleFor(o => o.FirstName, f => f.Name.FirstName())
               .RuleFor(o => o.LastName, f => f.Name.LastName());

            _userTable = new PagedTable<User>(() => b.Generate(460).ToArray(), 50);

            //

            _sortableUserTable = new SortablePagedTable<User>(() => b.Generate(460).ToArray(), 50);
            _sortableUserTable.AddSorter(nameof(User.FirstName), v => v.FirstName);
            _sortableUserTable.AddSorter(nameof(User.LastName), v => v.LastName);

            //

            _filterableSortableUserTable = new FilterableSortablePagedTable<User>(() => b.Generate(460).ToArray(), 50);
            _filterableSortableUserTable.AddSorter(nameof(User.FirstName), v => v.FirstName);
            _filterableSortableUserTable.AddSorter(nameof(User.LastName), v => v.LastName);

            _filterableSortableUserTable.AddMatcher(nameof(User.FirstName),
                (v, f) => v.FirstName.Contains(f, StringComparison.OrdinalIgnoreCase));

            _filterableSortableUserTable.AddMatcher(nameof(User.LastName),
                (v, f) => v.LastName.Contains(f, StringComparison.OrdinalIgnoreCase));

            Task.Run(() =>
            {
                Thread.Sleep(3000);
                _filterableSortableUserTable.SetFilter(nameof(User.FirstName), "ad");
            });
        }
    }
}