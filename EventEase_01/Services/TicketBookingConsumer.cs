using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using EventEase_01.Models;

namespace EventEase_01.Services
{
    public class TicketBookingConsumer : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly EventEase01Context _context;

        public TicketBookingConsumer(ConnectionFactory factory,EventEase01Context context)
        {
            _factory = factory;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "ticket-booking",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var ticketId = Guid.Parse(Encoding.UTF8.GetString(body));

                    if (_context.Tickets.Any(t => t.TicketId == ticketId && t.TicketAvailability == 0))
                    {
                        
                        Console.WriteLine($"Ticket {ticketId} already booked.");
                    }
                    else
                    {
                        var ticket = _context.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
                        if (ticket != null)
                        {
                            ticket.TicketAvailability = 0;
                            _context.SaveChanges();
                            Console.WriteLine($"Booking ticket {ticketId}");
                        }
                        
                    }
                };
                channel.BasicConsume(queue: "ticket-booking",
                                     autoAck: true,
                                     consumer: consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    }

}
