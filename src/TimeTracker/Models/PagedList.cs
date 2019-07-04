using System;
using System.Collections.Generic;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single page of items with additional paging information like
    /// page size, total count and total page size.
    /// </summary>
    /// <typeparam name="T">Type of the item in the list.</typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// Gets or sets one page of items.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total item count.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets the total page count.
        /// </summary>
        public int TotalPages => (int) Math.Ceiling((double) TotalCount / PageSize);
    }
}
