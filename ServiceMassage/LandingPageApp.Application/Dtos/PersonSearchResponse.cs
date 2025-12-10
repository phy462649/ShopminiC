using System.Collections.Generic;

namespace LandingPageApp.Application.Dtos
{
    public class PersonSearchResponse
    {
        /// <summary>
        /// List of persons matching the search criteria
        /// </summary>
        public IEnumerable<object> Items { get; set; } = new List<object>();

        /// <summary>
        /// Total number of items matching the search criteria (without pagination)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Search criteria used
        /// </summary>
        public PersonSearchRequest? SearchCriteria { get; set; }
    }
}
