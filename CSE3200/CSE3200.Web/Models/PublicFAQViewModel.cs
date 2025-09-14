using CSE3200.Domain.Entities;
using System.Collections.Generic;

namespace CSE3200.Web.Models
{
    public class PublicFAQViewModel
    {
        public IList<FAQ> FAQs { get; set; }
        public FAQCategory? SelectedCategory { get; set; }
        public string SearchTerm { get; set; }
        public FAQCategory[] Categories { get; set; }
    }
}