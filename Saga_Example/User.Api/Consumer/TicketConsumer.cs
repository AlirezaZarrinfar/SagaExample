using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using User.Api.Context;
using User.Api.Models;

namespace User.Api.Consumer
{
    public class TicketConsumer : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPublisher _publisher;

        public TicketConsumer(ISubscriber subscriber, IServiceScopeFactory scopeFactory, IPublisher publisher)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
            _publisher = publisher;
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
                var ticket = JsonConvert.DeserializeObject<TicketModel>(message);
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<UserContext>();
                var user = context.Users.Find(ticket.UserId);
                if (user.Age < 18 || user.Balance < ticket.Cost)
                {
                    throw new Exception(ticket.Id.ToString());
                }
                user.Balance -= (float)ticket.Cost;
                context.SaveChanges();
            }
            catch(Exception e)
            {
                if (Convert.ToInt32(e.Message) != 0)
                    _publisher.Publish(e.Message , "user_routingkey",null);
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
