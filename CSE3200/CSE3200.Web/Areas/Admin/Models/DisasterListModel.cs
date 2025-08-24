using CSE3200.Domain;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DisasterListModel : DataTables
    {
        public int Draw { get; set; }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                "Title",
                "Location",
                "Severity",
                "OccurredDate",
                "AffectedPeople",
                "Status"
            );
        }
    }
}
