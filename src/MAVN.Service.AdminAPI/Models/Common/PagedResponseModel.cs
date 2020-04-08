using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Common
{
    /// <summary>
    /// Model that contains base information for pagination
    /// </summary>
    [PublicAPI]
    public class PagedResponseModel
    {
        public PagedResponseModel()
        {
        }

        public PagedResponseModel(int currentPage, int totalCount)
        {
            CurrentPage = currentPage;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total count of records
        /// </summary>
        public int TotalCount { get; set; }
    }
}
