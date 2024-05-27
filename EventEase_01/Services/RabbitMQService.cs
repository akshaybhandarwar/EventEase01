//using RabbitMQ.Client.Events;
//using RabbitMQ.Client;
//using System.Text;
//using System.Text.Json.Nodes;
//using Newtonsoft.Json;

//namespace EventEase_01.Services
//{

//    public class RabbitMQService
//    {
//        private readonly ConnectionFactory _factory;
//        private readonly IConnection _connection;
//        private readonly IModel _channel;

//        public RabbitMQService()
//        {
//            _factory = new ConnectionFactory() { HostName = "5614-LAP-0344" };
//            _connection = _factory.CreateConnection();
//            _channel = _connection.CreateModel();
//            _channel.QueueDeclare(queue: "ticket_updates", durable: true, exclusive: false, autoDelete: false, arguments: null);
//        }

//        public void PublishMessage(string message)
//        {
//            var body = Encoding.UTF8.GetBytes(message);
//            _channel.BasicPublish(exchange: "", routingKey: "ticket_updates", basicProperties: null, body: body);
//        }
//        public void PublishBatch(List<Guid> ticketIds)
//        {
//            var message = JsonConvert.SerializeObject(ticketIds);
//            var body = Encoding.UTF8.GetBytes(message);
//            _channel.BasicPublish(exchange: "", routingKey: "ticket_updates", basicProperties: null, body: body);
//        }
//    }


//    public class TicketUpdateConsumer
//    {
//        private readonly RabbitMQService _rabbitMQService;
//        public TicketUpdateConsumer()
//        {
//            _rabbitMQService = new RabbitMQService();
//        }
//        //public void ConsumeTicketUpdateMessages()
//        //{
//        //    var factory = new ConnectionFactory() { HostName = "5614-LAP-0344" };
//        //    using (var connection = factory.CreateConnection())
//        //    using (var channel = connection.CreateModel())
//        //    {
//        //        channel.QueueDeclare(queue: "ticket_updates", durable: true, exclusive: false, autoDelete: false, arguments: null);
//        //        var consumer = new EventingBasicConsumer(channel);
//        //        consumer.Received += (model, ea) =>
//        //        {
//        //            var body = ea.Body.ToArray();
//        //            var ticketId = Encoding.UTF8.GetString(body);
//        //            ProcessTicketUpdate(ticketId);
//        //        };
//        //        channel.BasicConsume(queue: "ticket_updates", autoAck: true, consumer: consumer);
//        //    }
//        //}


//    }
//}