using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tables
{
    public partial class FilterableSortablePagedTable<TRow> : IFilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly PagedTable<TRow>.RowGetter _rowGetter;
        private readonly SortablePagedTable<TRow>   _table;

        public FilterableSortablePagedTable(PagedTable<TRow>.RowGetter rowGetter, int pageSize)
        {
            _rowGetter = rowGetter;

            _invalidateHook = new Hook();
            _paginateHook   = new Hook();
            _refreshHook    = new Hook();
            _sortHook       = new Hook();
            _filterHook     = new Hook();

            _table = new SortablePagedTable<TRow>
            (
                RowsFiltered, pageSize,
                _invalidateHook, _paginateHook, _refreshHook, _sortHook
            );
        }
    }

    public partial class FilterableSortablePagedTable<TRow> : IFilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly Hook _invalidateHook;
        private readonly Hook _paginateHook;
        private readonly Hook _refreshHook;
        private readonly Hook _sortHook;
        private readonly Hook _filterHook;

        public IPagedTable<TRow> BaseTable => _table.BaseTable;

        public int PageSize    => _table.PageSize;
        public int CurrentPage => _table.CurrentPage;
        public int NumPages    => _table.NumPages;

        public TRow[] RowsInView() => _table.RowsInView();

        public ReadOnlyHook OnInvalidate => _invalidateHook;
        public ReadOnlyHook OnPaginate   => _paginateHook;
        public ReadOnlyHook OnRefresh    => _refreshHook;
        public ReadOnlyHook OnSort       => _sortHook;
        public ReadOnlyHook OnFilter     => _filterHook;

        public string?             ActiveSorter        => _table.ActiveSorter;
        public TableSortDirection? ActiveSortDirection => _table.ActiveSortDirection;

        public bool CanPrevious()   => _table.CanPrevious();
        public bool CanNext()       => _table.CanNext();
        public void Previous()      => _table.Previous();
        public void Next()          => _table.Next();
        public void Jump(int index) => _table.Jump(index);

        public TRow[] RowsSorted() => _table.RowsSorted();
        public void   InvalidateRowsSorted() => _table.InvalidateRowsSorted();
        public void   NextSortDirection(string id) => _table.NextSortDirection(id);
        public void   AddSorter(string id, Func<TRow, IComparable> rowComparer) => _table.AddSorter(id, rowComparer);
        public void   Sort(string id, TableSortDirection direction) => _table.Sort(id, direction);
        public void   UnSort() => _table.UnSort();
    }

    public partial class FilterableSortablePagedTable<TRow> : IFilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly object _rowsLock         = new();
        private readonly object _rowsFilteredLock = new();

        private TRow[]? _rows;
        private TRow[]? _rowsFiltered;

        public TRow[] Rows()
        {
            Console.WriteLine("Rows()");
            lock (_rowsLock)
            {
                return _rows ??= _rowGetter.Invoke();
            }
        }

        public void InvalidateRows()
        {
            lock (_rowsFilteredLock)
            {
                _rows         = null;
                _rowsFiltered = null;

                _table.InvalidateRows();
            }
        }

        public void InvalidateRowsFiltered()
        {
            lock (_rowsFilteredLock)
            {
                _rowsFiltered = null;

                _table.InvalidateRows();
            }
        }

        public TRow[] RowsFiltered()
        {
            lock (_rowsFilteredLock)
            {
                if (_rowsFiltered != null)
                    return _rowsFiltered;

                TRow[] rows = Rows();

                lock (_matcherLock)
                {
                    foreach ((string key, Func<TRow, string, bool> matcher) in _matchers)
                    {
                        if (!_filters.TryGetValue(key, out string? filter))
                            continue;

                        rows = rows.Where(v => matcher.Invoke(v, filter)).ToArray();
                    }

                    _rowsFiltered = rows;
                    return _rowsFiltered;
                }
            }
        }

        public void Refresh()
        {
            _refreshHook.Invoke();
        }
    }

    public partial class FilterableSortablePagedTable<TRow> : IFilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly Dictionary<string, Func<TRow, string, bool>> _matchers    = new();
        private readonly object                                       _matcherLock = new();
        private readonly Dictionary<string, string>                   _filters     = new();

        public void AddMatcher(string id, Func<TRow, string, bool> matcher)
        {
            lock (_matcherLock)
            {
                _matchers.Add(id, matcher);
            }
        }

        public void SetFilter(string key, string filter, bool doUpdate = true)
        {
            lock (_matcherLock)
            {
                if (string.IsNullOrEmpty(filter))
                    throw new ArgumentException("Filter cannot be empty.", nameof(filter));

                if (!_matchers.ContainsKey(key))
                    throw new ArgumentException("Key does not have a registered matcher.", nameof(key));

                _filters[key] = filter;

                if (doUpdate) OnFilterChange?.Invoke(key, filter);
            }

            InvalidateRowsFiltered();
            _filterHook.Invoke();
        }

        public void ClearFilter(string key, bool doUpdate = true)
        {
            lock (_matcherLock)
            {
                if (!_matchers.ContainsKey(key))
                    throw new ArgumentException("Key does not have a registered matcher.", nameof(key));

                _filters.Remove(key);

                if (doUpdate) OnFilterChange?.Invoke(key, null);
            }

            InvalidateRowsFiltered();
            _filterHook.Invoke();
        }

        public string? GetFilter(string key)
        {
            _filters.TryGetValue(key, out string? filter);
            return filter;
        }

        public event Action<string, string?>? OnFilterChange;
    }
}