using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs
{
    public partial class PagedTable<TRow> where TRow : class
    {
        public delegate TRow[] RowGetter();

        private readonly RowGetter _rowGetter;

        public PagedTable(RowGetter rowGetter, int pageSize)
        {
            PageSize   = pageSize;
            _rowGetter = rowGetter;
        }
    }

    public partial class PagedTable<TRow>
    {
        private readonly object  _rowLock = new();
        private          TRow[]? _rows;

        public void InvalidateRows()
        {
            lock (_rowLock)
            {
                _rows = null;
            }
        }

        internal TRow[] Rows()
        {
            lock (_rowLock)
            {
                return _rows ??= _rowGetter.Invoke();
            }
        }
    }

    public partial class PagedTable<TRow>
    {
        private Action? _stateHasChanged;

        public int PageSize    { get; }
        public int CurrentPage { get; private set; }

        public int NumPages
        {
            get
            {
                lock (_rowLock)
                {
                    _rows ??= _rowGetter.Invoke();
                    return (int) Math.Ceiling(_rows.Length / (decimal) PageSize);
                }
            }
        }

        public event Action? OnPaginate;

        public List<TRow> RowsInView()
        {
            return Rows().Skip(CurrentPage * PageSize).Take(PageSize).ToList();
        }

        internal void Previous()
        {
            if (CanPrevious())
            {
                CurrentPage--;
                _stateHasChanged?.Invoke();
                OnPaginate?.Invoke();
            }
        }

        internal void Next()
        {
            if (CanNext())
            {
                CurrentPage++;
                _stateHasChanged?.Invoke();
                OnPaginate?.Invoke();
            }
        }

        internal void Jump(int index)
        {
            if (0 <= index && index < NumPages)
            {
                CurrentPage = index;
                _stateHasChanged?.Invoke();
                OnPaginate?.Invoke();
            }
        }

        internal bool CanPrevious() => CurrentPage - 1 >= 0;
        internal bool CanNext()     => CurrentPage + 1 < NumPages;

        internal void SetStateHasChanged(Action stateHasChanged)
        {
            _stateHasChanged = stateHasChanged;
        }
    }

    public partial class PagedTable<TRow> { }

    public class PagedTableButtons<TRow> : ComponentBase where TRow : class
    {
        private Button _previous = null!;
        private Button _next     = null!;

        [Parameter] public PagedTable<TRow> Table { get; set; } = null!;

        protected override void OnParametersSet()
        {
            Table.SetStateHasChanged(() => InvokeAsync(StateHasChanged));

            _previous = new Button(() => "PREVIOUS".AsContent(), new Button.Spec
            {
                IsDisabled = () => !Table.CanPrevious(),
                OnClick    = (_, _) => Table.Previous(),
            });

            _next = new Button(() => "NEXT".AsContent(), new Button.Spec
            {
                IsDisabled = () => !Table.CanNext(),
                OnClick    = (_, _) => Table.Next(),
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            IEnumerable<int> pages = Paginator.Pages(Table.NumPages, Table.CurrentPage);

            //

            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-PagedTable-PagedTableButtons");

            builder.AddContent(++seq, _previous.Renderer());

            foreach (int page in pages)
            {
                if (page != -1)
                {
                    builder.OpenElement(++seq, "button");
                    builder.AddAttribute(++seq, "disabled", Table.CurrentPage == page);
                    builder.AddAttribute(++seq, "onclick",  EventCallback.Factory.Create(this, () => Table.Jump(page)));
                    builder.AddContent(++seq, page + 1);
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(++seq, "button");
                    builder.AddAttribute(++seq, "disabled", true);
                    builder.AddAttribute(++seq, "class",    "I4E-Construct-PagedTable-Ellipses");
                    builder.AddContent(++seq, "...");
                    builder.CloseElement();
                }
            }

            builder.AddContent(++seq, _next.Renderer());

            builder.CloseElement();
        }
    }

    public class PagedTableUpdatableContent<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public PagedTable<TRow> Table        { get; set; } = null!;
        [Parameter] public RenderFragment   ChildContent { get; set; } = null!;

        protected override void OnParametersSet() => Table.OnPaginate += () => InvokeAsync(StateHasChanged);

        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
    }

    public class PagedTablePageInfo<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public PagedTable<TRow> Table { get; set; } = null!;

        protected override void OnParametersSet() => Table.OnPaginate += () => InvokeAsync(StateHasChanged);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            string info = "Showing "                                                                                  +
                          $"{Table.CurrentPage * Table.NumPages + 1} to "                                             +
                          $"{Math.Min(Table.CurrentPage * Table.NumPages + Table.PageSize, Table.Rows().Length)} of " +
                          $"{Table.Rows().Length:#,##0} | "                                                           +
                          $"{Table.NumPages} page"                                                                    +
                          $"{(Table.NumPages != 1 ? "s" : "")}";
            
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Construct-PagedTable-PageInfo");
            builder.AddContent(2, info);
            builder.CloseElement();
        }
    }
}