using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tables
{
    public partial class SortablePagedTable<TRow> : ISortablePagedTable<TRow> where TRow : class
    {
        private readonly PagedTable<TRow>.RowGetter _rowGetter;
        private readonly PagedTable<TRow>           _table;

        public SortablePagedTable(PagedTable<TRow>.RowGetter rowGetter, int pageSize)
        {
            _rowGetter = rowGetter;

            _invalidateHook = new Hook();
            _paginateHook   = new Hook();
            _refreshHook    = new Hook();
            _sortHook       = new Hook();

            _table = new PagedTable<TRow>(RowsSorted, pageSize, _invalidateHook, _paginateHook, _refreshHook);
        }

        internal SortablePagedTable
        (
            PagedTable<TRow>.RowGetter rowGetter,      int  pageSize,
            Hook                       invalidateHook, Hook paginateHook, Hook refreshHook, Hook sortHook
        )
        {
            _rowGetter = rowGetter;

            _invalidateHook = invalidateHook;
            _paginateHook   = paginateHook;
            _refreshHook    = refreshHook;
            _sortHook       = sortHook;

            _table = new PagedTable<TRow>(RowsSorted, pageSize, _invalidateHook, _paginateHook, _refreshHook);
        }
    }

    public partial class SortablePagedTable<TRow> where TRow : class
    {
        private readonly Hook _invalidateHook;
        private readonly Hook _paginateHook;
        private readonly Hook _refreshHook;
        private readonly Hook _sortHook;

        public IPagedTable<TRow> BaseTable => _table;

        public int PageSize    => _table.PageSize;
        public int CurrentPage => _table.CurrentPage;
        public int NumPages    => _table.NumPages;

        public TRow[] RowsInView() => _table.RowsInView();

        public ReadOnlyHook OnInvalidate => _invalidateHook;
        public ReadOnlyHook OnPaginate   => _paginateHook;
        public ReadOnlyHook OnRefresh    => _refreshHook;
        public ReadOnlyHook OnSort       => _sortHook;

        public bool CanPrevious()   => _table.CanPrevious();
        public bool CanNext()       => _table.CanNext();
        public void Previous()      => _table.Previous();
        public void Next()          => _table.Next();
        public void Jump(int index) => _table.Jump(index);
    }

    public partial class SortablePagedTable<TRow> where TRow : class
    {
        private readonly object _rowsLock       = new();
        private readonly object _rowsSortedLock = new();

        private TRow[]? _rows;
        private TRow[]? _rowsSorted;

        public TRow[] Rows()
        {
            lock (_rowsLock)
            {
                return _rows ??= _rowGetter.Invoke();
            }
        }

        public void InvalidateRows()
        {
            lock (_rowsSortedLock)
            {
                _rows       = null;
                _rowsSorted = null;

                _table.InvalidateRows();
            }
        }

        public void InvalidateRowsSorted()
        {
            lock (_rowsSortedLock)
            {
                _rowsSorted = null;

                _table.InvalidateRows();
            }
        }

        public TRow[] RowsSorted()
        {
            lock (_rowsSortedLock)
            {
                if (_rowsSorted != null)
                    return _rowsSorted;

                TRow[] rows = Rows();

                lock (_sortLock)
                {
                    _rowsSorted = ActiveSorter == null
                        ? _rows
                        : ActiveSortDirection == TableSortDirection.Ascending
                            ? rows.OrderBy(k => _sortComparers[ActiveSorter].Invoke(k)).ToArray()
                            : rows.OrderByDescending(k => _sortComparers[ActiveSorter].Invoke(k)).ToArray();

                    return _rowsSorted!;
                }
            }
        }

        public void Refresh()
        {
            _refreshHook.Invoke();
        }
    }

    public enum TableSortDirection
    {
        Ascending, Descending,
    };

    public partial class SortablePagedTable<TRow> where TRow : class
    {
        private readonly Dictionary<string, Func<TRow, IComparable>> _sortComparers = new();
        private readonly object                                      _sortLock      = new();

        public TableSortDirection? ActiveSortDirection { get; private set; }

        public string? ActiveSorter { get; private set; }

        public void NextSortDirection(string id)
        {
            Console.Write($"{id} > {ActiveSorter}, {ActiveSortDirection} -> ");
            lock (_sortLock)
            {
                if (ActiveSorter != id)
                {
                    ActiveSorter        = id;
                    ActiveSortDirection = TableSortDirection.Ascending;
                }
                else
                {
                    if (ActiveSortDirection == TableSortDirection.Ascending)
                    {
                        ActiveSortDirection = TableSortDirection.Descending;
                    }
                    else
                    {
                        ActiveSorter        = null;
                        ActiveSortDirection = null;
                    }
                }
            }

            InvalidateRowsSorted();
            // _sortHook.Invoke();
        }

        public void AddSorter(string id, Func<TRow, IComparable> rowComparer)
        {
            lock (_sortLock)
            {
                _sortComparers[id] = rowComparer;
            }
        }

        public void Sort(string id, TableSortDirection direction)
        {
            lock (_sortLock)
            {
                ActiveSorter        = id;
                ActiveSortDirection = direction;
            }

            InvalidateRowsSorted();
            // _sortHook.Invoke();
        }

        public void UnSort()
        {
            lock (_sortLock)
            {
                ActiveSorter        = null;
                ActiveSortDirection = null;
            }

            InvalidateRowsSorted();
            // _sortHook.Invoke();
        }
    }
}