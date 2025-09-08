using CSE3200.Application.Features.DisasterAlerts.Queries;
using CSE3200.Domain.Entities;
using CSE3200.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE3200.Web.Controllers
{
    public class PublicDisasterAlertController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PublicDisasterAlertController> _logger;

        public PublicDisasterAlertController(
            IMediator mediator,
            ILogger<PublicDisasterAlertController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> GetCurrentAlerts()
        {
            try
            {
                var query = new GetDisasterAlertsQuery
                {
                    CurrentOnly = true
                };

                var alerts = await _mediator.Send(query);

                return Ok(alerts);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading current disaster alerts");
                return Ok(new List<DisasterAlert>());
            }
        }

        public IActionResult AlertBar()
        {
            return View();
        }
    }
}