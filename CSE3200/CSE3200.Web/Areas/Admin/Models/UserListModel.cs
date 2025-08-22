using CSE3200.Domain;
using System.Collections.Generic;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class UserListModel : DataTables
    {
        public int Draw { get; set; }

        public string RoleFilter { get; set; }
        public string EmailFilter { get; set; }

        public List<SortColumn> OrderList
        {
            get => Order != null ? new List<SortColumn>(Order) : new List<SortColumn>();
            set => Order = value?.ToArray();
        }

        public string GetSortExpression()
        {
            return FormatSortExpression(
                "FirstName",
                "LastName",
                "Email",
                "UserName",
                "DateOfBirth",
                "RegistrationDate"
            );
        }

        public override string ToString()
        {
            return $"Draw: {Draw}, Start: {Start}, Length: {Length}, Search: {Search.Value}, Role: {RoleFilter}, Email: {EmailFilter}";
        }

        public bool IsValid(out string errorMessage)
        {
            if (PageSize < 1 || PageSize > 100)
            {
                errorMessage = "PageSize must be between 1 and 100";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public bool ShouldApplyEmailFilter() => !string.IsNullOrWhiteSpace(EmailFilter);
        public bool ShouldApplyRoleFilter() => !string.IsNullOrWhiteSpace(RoleFilter);
    }
}
