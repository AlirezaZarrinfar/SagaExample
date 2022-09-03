using Microsoft.EntityFrameworkCore;

namespace Ticket.Api.Context
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions options) : base(options)
        { }
        public DbSet<Entities.Ticket> Tickets { get; set; }
    }
}
