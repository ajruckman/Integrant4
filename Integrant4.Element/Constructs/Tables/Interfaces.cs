using System;
using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tables
{
    public interface IPagedTable<out TRow> where TRow : class
    {
        public IPagedTable<TRow> BaseTable { get; }

        public int PageSize    { get; }
        public int CurrentPage { get; }
        public int NumPages    { get; }

        public ReadOnlyHook OnInvalidate { get; }
        public ReadOnlyHook OnPaginate   { get; }
        public ReadOnlyHook OnRefresh    { get; }

        public TRow[] Rows();
        public TRow[] RowsInView();
        public void   InvalidateRows();
        public void   Refresh();

        public bool CanPrevious();
        public bool CanNext();
        public void Previous();
        public void Next();
        public void Jump(int index);
    }

    public interface ISortablePagedTable<out TRow> : IPagedTable<TRow> where TRow : class
    {
        public new IPagedTable<TRow> BaseTable { get; }
        public     ReadOnlyHook      OnSort    { get; }

        public string?             ActiveSorter        { get; }
        public TableSortDirection? ActiveSortDirection { get; }

        public TRow[] RowsSorted();
        public void   InvalidateRowsSorted();

        public void NextSortDirection(string id);

        public void AddSorter(string id, Func<TRow, IComparable> rowComparer);
        public void Sort(string      id, TableSortDirection      direction);
        public void UnSort();
    }

    public interface IFilterableSortablePagedTable<out TRow> : ISortablePagedTable<TRow> where TRow : class
    {
        public new IPagedTable<TRow> BaseTable { get; }
        public     ReadOnlyHook      OnFilter  { get; }

        public TRow[] RowsFiltered();
        public void   InvalidateRowsFiltered();

        public void    AddMatcher(string  id,  Func<TRow, string, bool> matcher);
        public void    SetFilter(string   key, string                   filter, bool doUpdate = true);
        public void    ClearFilter(string key, bool                     doUpdate = true);
        public string? GetFilter(string   key);

        public event Action<string, string?>? OnFilterChange;
    }
}