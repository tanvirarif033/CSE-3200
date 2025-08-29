using CSE3200.Domain;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DonationListModel : DataTables
    {
        public int Draw { get; set; }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                "DonorName",
                "DonorEmail",
                "Amount",
                "PaymentMethod",
                "PaymentStatus",
                "DonationDate",
                "Disaster.Title"
            );
        }
    }
}