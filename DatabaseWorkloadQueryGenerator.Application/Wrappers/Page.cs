using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Wrappers
{

    /// <summary>
    /// Represents the result of a paginated request
    /// </summary>
    /// <typeparam name="T">The type of the items data</typeparam>
    public class Page<T>
    {
        /// <summary>
        /// The list of items in the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = [];

        /// <summary>
        /// The page number, in 1 based index
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages available
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

        /// <summary>
        /// Total number of records available
        /// </summary>
        public int TotalRecords { get; set; }
    }
}