using System;
using System.Linq;
using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tables
{
    public partial class PagedTable<TRow> : IPagedTable<TRow> where TRow : class
    {
        public delegate TRow[] RowGetter();

        private readonly RowGetter _rowGetter;

        public PagedTable(RowGetter rowGetter, int pageSize)
        {
            _rowGetter = rowGetter;
            PageSize   = pageSize;

            _invalidateHook = new Hook();
            _paginateHook   = new Hook();
            _refreshHook    = new Hook();
        }

        internal PagedTable(RowGetter rowGetter, int pageSize, Hook invalidateHook, Hook paginateHook, Hook refreshHook)
        {
            _rowGetter = rowGetter;
            PageSize   = pageSize;

            _invalidateHook = invalidateHook;
            _paginateHook   = paginateHook;
            _refreshHook    = refreshHook;
        }

        public IPagedTable<TRow> BaseTable => this;
    }

    public partial class PagedTable<TRow>
    {
        private readonly object _rowsLock = new();

        private TRow[]? _rows;

        public TRow[] Rows()
        {
            lock (_rowsLock)
            {
                if (_rows == null)
                {
                    _rows = _rowGetter.Invoke();

                    var numPages = (int) Math.Ceiling(_rows.Length / (decimal) PageSize);

                    if (CurrentPage > numPages)
                        CurrentPage = numPages - 1;
                }

                return _rows;
            }
        }

        public void InvalidateRows()
        {
            lock (_rowsLock)
            {
                _rows = null;
            }

            _invalidateHook.Invoke();
        }

        public void Refresh()
        {
            _refreshHook.Invoke();
        }
    }

    public partial class PagedTable<TRow>
    {
        private readonly Hook _invalidateHook;
        private readonly Hook _paginateHook;
        private readonly Hook _refreshHook;

        public ReadOnlyHook OnInvalidate => _invalidateHook;
        public ReadOnlyHook OnPaginate   => _paginateHook;
        public ReadOnlyHook OnRefresh    => _refreshHook;

        public int PageSize    { get; }
        public int CurrentPage { get; private set; }

        public int NumPages
        {
            get
            {
                lock (_rowsLock)
                {
                    if (_rows == null)
                    {
                        _rows = _rowGetter.Invoke();
                    }

                    var numPages = (int) Math.Ceiling(_rows.Length / (decimal) PageSize);

                    if (CurrentPage > numPages)
                        CurrentPage = numPages - 1;

                    return numPages;
                }
            }
        }

        public TRow[] RowsInView()
        {
            return Rows().Skip(CurrentPage * PageSize).Take(PageSize).ToArray();
        }

        public void Previous()
        {
            if (CanPrevious())
            {
                CurrentPage--;
                _paginateHook.Invoke();
            }
        }

        public void Next()
        {
            if (CanNext())
            {
                CurrentPage++;
                _paginateHook.Invoke();
            }
        }

        public void Jump(int index)
        {
            if (0 <= index && index < NumPages)
            {
                CurrentPage = index;
                _paginateHook.Invoke();
            }
        }

        public bool CanPrevious() => CurrentPage - 1 >= 0;
        public bool CanNext()     => CurrentPage + 1 < NumPages;
    }
}