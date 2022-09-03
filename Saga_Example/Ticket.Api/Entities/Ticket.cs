namespace Ticket.Api.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Cost { get; set; }
        public int UserId { get; set; }
    }
}
