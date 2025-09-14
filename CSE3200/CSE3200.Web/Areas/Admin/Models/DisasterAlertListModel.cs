using CSE3200.Domain;
using System.Collections.Generic;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DisasterAlertListModel : DataTables
    {
        public int Draw { get; set; }
        public string? SearchValue { get; set; }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                "Title",
                "Severity",
                "StartDate",
                "EndDate",
                "DisplayOrder",
                "IsActive",
                "CreatedDate"
            );
        }

        public static DisasterAlertListModel FromDataTablesRequest(int draw, int start, int length, string searchValue, List<SortColumn> order)
        {
            return new DisasterAlertListModel
            {
                Draw = draw,
                Start = start,
                Length = length,
                SearchValue = searchValue,
                Order = order.ToArray(),
                Search = new DataTablesSearch { Value = searchValue }
            };
        }
    }
}