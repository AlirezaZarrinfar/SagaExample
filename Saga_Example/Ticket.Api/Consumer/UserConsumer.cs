using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plain.RabbitMQ;
using Ticket.Api.Context;

namespace Ticket.Api.Consumer
{
    public class UserConsumer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISubscriber _subscriber;
        public UserConsumer(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        bool Subscribe(string message, IDictionary<string, object> header)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<TicketContext>();
                var ticket = context.Tickets.Find(Convert.ToInt32(message));
                context.Tickets.Remove(ticket);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
