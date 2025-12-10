using System.ComponentModel.DataAnnotations;

namespace LandingPageApp.Application.Dtos
{
    public class PersonSearchRequest
    {
        /// <summary>
        /// Keyword to search in Name, Email, Phone, Username fields
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filter by Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Filter by Email
        /// </summary>
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Filter by Phone
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Filter by Username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Filter by Role ID
        /// </summary>
        public long? RoleId { get; set; }

        /// <summary>
        /// Filter by creation date from (inclusive)
        /// </summary>
        public DateTime? CreatedFrom { get; set; }

        /// <summary>
        /// Filter by creation date to (inclusive)
        /// </summary>
        public DateTime? CreatedTo { get; set; }

        /// <summary>
        /// Sort by field (Name, Email, Phone, Username, CreatedAt, UpdatedAt)
        /// </summary>
        public string? SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sort order (asc, desc)
        /// </summary>
        public string? SortOrder { get; set; } = "desc";

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
