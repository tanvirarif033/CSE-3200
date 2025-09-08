using CSE3200.Domain;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DisasterAlertListModel : DataTables
    {
        public int Draw { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

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
    }
}