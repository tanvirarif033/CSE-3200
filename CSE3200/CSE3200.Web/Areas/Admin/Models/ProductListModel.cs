using CSE3200.Domain;
using System;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class ProductListModel : DataTables
    {
        public const int NameSortColumn = 0;
        public const int CategorySortColumn = 1;
        public const int PriceSortColumn = 2;
        public const int RatingSortColumn = 3;
        public const int QuantitySortColumn = 4;
        public const int IdSortColumn = 5;

        public string CategoryFilter { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinRating { get; set; }
        public int Draw { get; set; }



        public override string ToString()
        {
            return $"PageIndex: {PageIndex}, PageSize: {PageSize}, " +
                   $"Search: {Search.Value}, " +
                   $"Sort: {GetSortExpression()}, " +
                   $"CategoryFilter: {CategoryFilter}, " +
                   $"PriceRange: {MinPrice}-{MaxPrice}, " +
                   $"MinRating: {MinRating}";
        }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                nameof(NameSortColumn),
                nameof(CategorySortColumn),
                nameof(PriceSortColumn),
                nameof(RatingSortColumn),
                nameof(QuantitySortColumn),
                nameof(IdSortColumn)
            );
        }


        public bool IsValid(out string errorMessage)
        {
            if (PageSize < 1 || PageSize > 100)
            {
                errorMessage = "PageSize must be between 1 and 100";
                return false;
            }

            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            {
                errorMessage = "MinPrice cannot be greater than MaxPrice";
                return false;
            }

            errorMessage = null;
            return true;
        }


        public bool ShouldApplyCategoryFilter()
        {
            return !string.IsNullOrWhiteSpace(CategoryFilter);
        }

        public bool ShouldApplyPriceFilter()
        {
            return MinPrice.HasValue || MaxPrice.HasValue;
        }

        public bool ShouldApplyRatingFilter()
        {
            return MinRating.HasValue;
        }
        public static ProductListModel FromDataTablesRequest(int start, int length, string searchValue, List<SortColumn> order)
        {
            return new ProductListModel
            {
                Start = start,
                Length = length,
                Search = new DataTablesSearch { Value = searchValue },
                Order = order.ToArray()
            };
        }
    }
}