using CSE3200.Domain;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class FAQListModel : DataTables
    {
        public int Draw { get; set; }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                "Question",
                "Category",
                "DisplayOrder",
                "IsActive",
                "CreatedDate"
            );
        }
    }
}