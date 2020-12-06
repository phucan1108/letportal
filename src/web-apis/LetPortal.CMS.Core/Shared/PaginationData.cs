using System;
using System.Collections.Generic;

namespace LetPortal.CMS.Core.Shared
{
    public class PaginationData<T>
    {
        private readonly int _maximumPage;

        private readonly int _lastPage;

        public PaginationData(
            IReadOnlyList<T> data,
            long totalRecords,
            int currentPage,
            int numberPerPage,
            int maximumPage)
        {
            Data = data;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            NumberPerPage = numberPerPage;
            _maximumPage = maximumPage;
            _lastPage = (int)Math.Round((double)(TotalRecords / NumberPerPage), MidpointRounding.ToEven);
            TotalPage = _lastPage;
            // Detect all displayed pages
            var space = CurrentPage > _maximumPage ?
                        (int)Math.Round((double)(CurrentPage / _maximumPage), MidpointRounding.ToEven)
                            : 0;
            var startIndex = space * _maximumPage + 1;
            var lastIndex = (startIndex + _maximumPage - 1) >= _lastPage ? _lastPage : space * _maximumPage + _maximumPage;
            var length = lastIndex - startIndex + 1;
            DisplayedPages = new int[length];
            var j = 0;
            for (var i = startIndex; i <= lastIndex; i++)
            {
                DisplayedPages[j] = i;
                j++;
            }

            AllowNext = lastIndex < _lastPage;
            AllowPrevious = space > 0;
            if (AllowNext)
            {
                NextPage = (space + 1) * _maximumPage + 1;
            }

            if (AllowPrevious)
            {
                PreviousPage = (space - 1) * _maximumPage + _maximumPage;
            }
        }

        public IEnumerable<T> Data { get; }

        public long TotalRecords { get; }

        public int TotalPage { get; }

        public int CurrentPage { get; }

        public int NumberPerPage { get; }
        public int[] DisplayedPages { get; }
        public bool AllowNext { get; }
        public bool AllowPrevious { get; }

        public int NextPage { get; }

        public int PreviousPage { get; }
        public bool CanPaginate => TotalRecords > 0 && TotalPage > 1;

    }
}
