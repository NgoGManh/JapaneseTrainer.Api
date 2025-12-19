using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace JapaneseTrainer.Api.DTOs.Common
{
    /// <summary>
    /// Base class for paginated requests (following Python FastAPI pattern)
    /// Similar to PaginationParams in Python FastAPI
    /// </summary>
    public class PagedRequest
    {
        private int _page = 1;
        private int _limit = 20;

        /// <summary>
        /// Page number (1-based). Default: 1, must be greater than 0
        /// </summary>
        [FromQuery(Name = "page")]
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page. Default: 20, Max: 100, must be greater than 0
        /// </summary>
        [FromQuery(Name = "limit")]
        [DefaultValue(20)]
        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit
        {
            get => _limit;
            set => _limit = value < 1 ? 20 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Search term (searches across multiple fields). Anything you want.
        /// </summary>
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        /// <summary>
        /// List of field names to search in (if null, searches in default fields)
        /// Comma-separated string that will be parsed to list
        /// </summary>
        [FromQuery(Name = "search_in")]
        public string? SearchIn { get; set; }

        /// <summary>
        /// Field name to sort by. Default: "created_at". Anything you want.
        /// </summary>
        [FromQuery(Name = "sort_by")]
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort order: "asc" or "desc". Default: "desc"
        /// desc: Descending | asc: Ascending
        /// </summary>
        [FromQuery(Name = "order_by")]
        [DefaultValue("desc")]
        [RegularExpression("^(asc|desc)$", ErrorMessage = "OrderBy must be either 'asc' or 'desc'")]
        public string OrderBy { get; set; } = "desc";

        /// <summary>
        /// Comma-separated list of fields to include in the response (field selection)
        /// </summary>
        [FromQuery(Name = "fields")]
        public string? Fields { get; set; }

        /// <summary>
        /// Normalizes pagination values to ensure defaults are applied
        /// </summary>
        public void Normalize()
        {
            if (Page < 1) Page = 1;
            if (Limit < 1) Limit = 20;
            if (Limit > 100) Limit = 100;
            if (string.IsNullOrWhiteSpace(OrderBy) || (OrderBy.ToLower() != "asc" && OrderBy.ToLower() != "desc"))
            {
                OrderBy = "desc";
            }
            else
            {
                OrderBy = OrderBy.ToLower();
            }
        }
    }
}

