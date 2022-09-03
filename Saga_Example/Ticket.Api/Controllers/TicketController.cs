using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using Ticket.Api.Consumer;
using Ticket.Api.Context;

namespace Ticket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IPublisher _publisher;
        private readonly TicketContext _ticketContext;
        public TicketController(IPublisher publisher, TicketContext ticketContext)
        {
            _publisher = publisher;
            _ticketContext = ticketContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Entities.Ticket ticket)
        {
            await _ticketContext.Tickets.AddAsync(ticket);
            var res = await _ticketContext.SaveChangesAsync();
            var jsonTicket = JsonConvert.SerializeObject(ticket); 
            _publisher.Publish(jsonTicket, "ticket_routingkey" , null);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetTickets()
        {
            var res = _ticketContext.Tickets.ToList();
            return Ok(res);
        }

    }
}
