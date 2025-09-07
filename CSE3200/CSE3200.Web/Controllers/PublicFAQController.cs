using CSE3200.Application.Features.FAQs.Queries;
using CSE3200.Domain.Entities;
using CSE3200.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Web.Controllers
{
    public class PublicFAQController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PublicFAQController> _logger;

        public PublicFAQController(
            IMediator mediator,
            ILogger<PublicFAQController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string category = null, string search = null)
        {
            try
            {
                FAQCategory? faqCategory = null;
                if (!string.IsNullOrEmpty(category) && System.Enum.TryParse<FAQCategory>(category, out var parsedCategory))
                {
                    faqCategory = parsedCategory;
                }

                var query = new GetFAQsQuery
                {
                    Category = faqCategory,
                    SearchTerm = search
                };

                var faqs = await _mediator.Send(query);

                var model = new PublicFAQViewModel
                {
                    FAQs = faqs,
                    SelectedCategory = faqCategory,
                    SearchTerm = search,
                    Categories = System.Enum.GetValues<FAQCategory>()
                };

                return View(model);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading FAQs");
                return View(new PublicFAQViewModel
                {
                    FAQs = new List<FAQ>(),
                    Categories = System.Enum.GetValues<FAQCategory>()
                });
            }
        }
    }
}