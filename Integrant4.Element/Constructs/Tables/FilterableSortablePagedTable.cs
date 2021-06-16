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
            _reduceHook     = new Hook();

            _table = new SortablePagedTable<TRow>
            (
                RowsReduced, pageSize,
                _invalidateHook, _paginateHook, _refreshHook, _sortHook
            );
        }
    }

    public partial class FilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly Hook _invalidateHook;
        private readonly Hook _paginateHook;
        private readonly Hook _refreshHook;
        private readonly Hook _sortHook;
        private readonly Hook _reduceHook;

        public IPagedTable<TRow> BaseTable => _table.BaseTable;

        public int PageSize    => _table.PageSize;
        public int CurrentPage => _table.CurrentPage;
        public int NumPages    => _table.NumPages;

        public TRow[] RowsInView() => _table.RowsInView();

        public ReadOnlyHook OnInvalidate => _invalidateHook;
        public ReadOnlyHook OnPaginate   => _paginateHook;
        public ReadOnlyHook OnRefresh    => _refreshHook;
        public ReadOnlyHook OnSort       => _sortHook;
        public ReadOnlyHook OnReduce     => _reduceHook;

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

    public partial class FilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly object _rowsLock        = new();
        private readonly object _rowsReducedLock = new();

        private TRow[]? _rows;
        private TRow[]? _rowsReduced;

        public TRow[] Rows()
        {
            lock (_rowsLock)
            {
                return _rows ??= _rowGetter.Invoke();
            }
        }

        public void InvalidateRows()
        {
            lock (_rowsReducedLock)
            {
                _rows        = null;
                _rowsReduced = null;

                _table.InvalidateRows();
            }
        }

        public void InvalidateRowsReduced()
        {
            lock (_rowsReducedLock)
            {
                _rowsReduced = null;

                _table.InvalidateRows();
            }
        }

        public TRow[] RowsReduced()
        {
            lock (_rowsReducedLock)
            {
                if (_rowsReduced != null)
                    return _rowsReduced;

                IEnumerable<TRow> rows = Rows();

                lock (_reducerLock)
                {
                    foreach ((string key, Func<TRow, string, bool> filterer) in _filterers)
                    {
                        if (_appliedFilters.TryGetValue(key, out string? filter))
                        {
                            rows = rows.Where(v => filterer.Invoke(v, filter));
                        }
                    }

                    foreach ((string key, Func<TRow, bool> matcher) in _matchers)
                    {
                        if (_appliedMatchers.Contains(key))
                        {
                            rows = rows.Where(v => matcher.Invoke(v));
                        }
                    }

                    _rowsReduced = rows.ToArray();
                    return _rowsReduced;
                }
            }
        }

        public void Refresh()
        {
            _refreshHook.Invoke();
        }
    }

    public partial class FilterableSortablePagedTable<TRow> where TRow : class
    {
        private readonly object _reducerLock = new();

        private readonly Dictionary<string, Func<TRow, string, bool>> _filterers      = new();
        private readonly Dictionary<string, string>                   _appliedFilters = new();

        private readonly Dictionary<string, Func<TRow, bool>> _matchers        = new();
        private readonly HashSet<string>                      _appliedMatchers = new();

        public void AddFilterer(string id, Func<TRow, string, bool> filterer)
        {
            lock (_reducerLock)
            {
                _filterers.Add(id, filterer);
            }
        }

        public void SetFilter(string key, string filter, bool doUpdate = true)
        {
            lock (_reducerLock)
            {
                if (string.IsNullOrEmpty(filter))
                    throw new ArgumentException("Filter cannot be empty.", nameof(filter));

                if (!_filterers.ContainsKey(key))
                    throw new ArgumentException($"Key '{key}' does not have a registered filterer.", nameof(key));

                _appliedFilters[key] = filter;

                if (doUpdate) OnFilterChange?.Invoke(key, filter);
            }

            InvalidateRowsReduced();
            _reduceHook.Invoke();
        }

        public void ClearFilter(string key, bool doUpdate = true)
        {
            lock (_reducerLock)
            {
                if (!_filterers.ContainsKey(key))
                    throw new ArgumentException($"Key '{key}' does not have a registered filterer.", nameof(key));

                _appliedFilters.Remove(key);

                if (doUpdate) OnFilterChange?.Invoke(key, null);
            }

            InvalidateRowsReduced();
            _reduceHook.Invoke();
        }

        public void AddMatcher(string id, Func<TRow, bool> matcher)
        {
            lock (_reducerLock)
            {
                _matchers.Add(id, matcher);
            }
        }

        public void ApplyMatcher(string key, bool doUpdate = true)
        {
            lock (_reducerLock)
            {
                if (!_matchers.ContainsKey(key))
                    throw new ArgumentException($"Key '{key}' does not have a registered matcher.", nameof(key));

                _appliedMatchers.Add(key);

                if (doUpdate) OnReducerChange?.Invoke(key, true);
            }

            InvalidateRowsReduced();
            _reduceHook.Invoke();
        }

        public void UnapplyMatcher(string key, bool doUpdate = true)
        {
            lock (_reducerLock)
            {
                if (!_matchers.ContainsKey(key))
                    throw new ArgumentException($"Key '{key}' does not have a registered matcher.", nameof(key));

                _appliedMatchers.Remove(key);

                if (doUpdate) OnReducerChange?.Invoke(key, false);
            }

            InvalidateRowsReduced();
            _reduceHook.Invoke();
        }

        public string? GetFilter(string key)
        {
            _appliedFilters.TryGetValue(key, out string? filter);
            return filter;
        }

        public event Action<string, string?>? OnFilterChange;
        public event Action<string, bool>?    OnReducerChange;
    }
}