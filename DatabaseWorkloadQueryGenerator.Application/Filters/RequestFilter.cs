using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Filters
{

    /// <summary>
    /// A filter to apply when requesting for a paginated and filtered list of items
    /// </summary>
    public class RequestFilter :
        IPaginationFilter,
        IMultiSortFilter,
        ISearchFilter,
        IDateRangeFilter
    {

        #region Private Members

        private DateTime _endDate = DateTime.UtcNow;
        private DateTime _startDate = DateTime.UtcNow.AddDays(-7);
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private string _searchTerm = string.Empty;
        private bool _includeArchived;

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of the page to return
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// The size of the page to return
        /// </summary>
        [Range(0, 10_000)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value;
        }

        /// <summary>
        /// Sorting Options
        /// </summary>
        public IList<SortOption> SortOptions { get; set; } = new List<SortOption>();

        /// <summary>
        /// A term used for searching
        /// </summary>
        public string SearchTerm
        {
            get => _searchTerm;
            set => _searchTerm = value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Minimum date to filter by
        /// </summary>
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = ValidateStartDate(value);
        }

        /// <summary>
        /// Maximum date to filter by
        /// </summary>
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = ValidateEndDate(value);
        }

        /// <summary>
        /// Whether to include archived items in the results
        /// </summary>
        public bool IncludeArchived
        {
            get => _includeArchived;
            set => _includeArchived = value;
        }

        #endregion

        #region Helper Methods

        public string BuildQueryString()
        {
            Validate();
            var queryBuilder = new StringBuilder("?");

            AddPagination(queryBuilder);
            AddSorting(queryBuilder);
            AddSearch(queryBuilder);
            AddDateRange(queryBuilder);
            AddArchivedFilter(queryBuilder);

            return queryBuilder.ToString().TrimEnd('&');
        }

        private void Validate()
        {
            if (StartDate > EndDate)
                StartDate = EndDate.AddDays(-1);
        }

        private void AddPagination(StringBuilder sb)
        {
            sb.Append($"pageNumber={PageNumber}&pageSize={PageSize}&");
        }

        private void AddSorting(StringBuilder sb)
        {
            if (SortOptions.Any())
            {
                var sortParams = SortOptions
                    .Select(so => $"{Uri.EscapeDataString(so.Field)}:{so.Direction.ToString().ToLower()}");

                sb.Append($"sort={string.Join(",", sortParams)}&");
            }
        }

        private void AddSearch(StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                sb.Append($"searchTerm={Uri.EscapeDataString(SearchTerm)}&");
            }
        }

        private void AddDateRange(StringBuilder sb)
        {
            sb.Append($"startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}&");
        }

        private void AddArchivedFilter(StringBuilder sb)
        {
            if (IncludeArchived)
            {
                sb.Append("includeArchived=true&");
            }
        }

        private DateTime ValidateStartDate(DateTime date)
        {
            return date < EndDate ? date : EndDate.AddDays(-1);
        }

        private DateTime ValidateEndDate(DateTime date)
        {
            return date > DateTime.UtcNow ? DateTime.UtcNow : date;
        }

        #endregion
    }


    public interface IDateRangeFilter
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }

    /// <summary>
    /// The direction to sort by
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Sort rows ascending
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort rows descending
        /// </summary>
        Descending
    }

    /// <summary>
    /// Specifies how to sort a property
    /// </summary>
    /// <param name="Field">The field name to sort</param>
    /// <param name="Direction">Whether to sort ascending or descending</param>
    public record SortOption(string Field, SortDirection Direction);

    public interface IPaginationFilter
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }

    public interface IMultiSortFilter
    {
        IList<SortOption> SortOptions { get; set; }
    }

    public interface ISearchFilter
    {
        string SearchTerm { get; set; }
    }
}
