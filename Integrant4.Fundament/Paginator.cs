using System;
using System.Collections.Generic;
using System.Linq;

namespace Integrant4.Fundament
{
    public static class Paginator
    {
        public static IEnumerable<int> Pages(int numPages, int currentPage)
        {
            const int radius   = 3;
            const int diameter = 2 * radius + 1;
            const int offset   = (int) (diameter / 2.0);

            List<int> pages = new();

            int start, end;

            if (numPages <= diameter)
            {
                start = 0;
                end   = Math.Max(numPages - 3, numPages);

                pages.AddRange(Enumerable.Range(start, end).ToList());
            }
            else if (currentPage <= offset)
            {
                start = 0;
                end   = diameter - 1;

                pages.AddRange(Enumerable.Range(start, end - 1).ToList());

                pages.Add(-1);
                pages.Add(numPages - 1);
            }
            else if (currentPage + offset >= numPages)
            {
                start = numPages - diameter;
                end   = numPages - 1;

                pages.Add(0);
                pages.Add(-1);

                pages.AddRange(Enumerable.Range(start + 2, end - start - 1).ToList());
            }
            else
            {
                start = currentPage - radius + 2;
                end   = currentPage + radius - 2;

                pages.Add(0);
                pages.Add(-1);

                pages.AddRange(Enumerable.Range(start, end - start + 1).ToList());

                if (currentPage == numPages - radius - 1)
                    pages.Add(numPages - 2);
                else
                    pages.Add(-1);

                pages.Add(numPages - 1);
            }

            return pages;
        }
    }
}