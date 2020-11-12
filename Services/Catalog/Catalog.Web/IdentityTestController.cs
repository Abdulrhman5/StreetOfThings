using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogService
{
    [Route("Test")]
    public class IdentityTestController : Controller
    {
        private IConfiguration _configs;

        private IEventBus _eventBus;
        public IdentityTestController(IConfiguration configs, IEventBus eventBus)
        {
            _configs = configs;
            _eventBus = eventBus;
        }

        // GET: /<controller>/
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return new JsonResult(new { Result = "Hello there" });
        }

        [HttpGet]
        [Route("publish")]
        public async Task<IActionResult> PublishEvent()
        {
            var evnt = new DummyEvent()
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                X = "Here we are"
            };

            _eventBus.Publish(evnt);
            return Ok();
        }
    }

    public class DummyEvent : IntegrationEvent
    {
        public string X { get; set; }
    }
}
