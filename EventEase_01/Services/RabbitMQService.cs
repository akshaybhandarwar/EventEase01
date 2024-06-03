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
//            _factory = new ConnectionFactory() { HostName = "localhost" };
//            _connection = _factory.CreateConnection();
//            _channel = _connection.CreateModel();
//            _channel.QueueDeclare(queue: "ticket_updates", durable: true, exclusive: false, autoDelete: false, arguments: null);
//            _channel.QueueDeclare(queue: "ticket_acknowledgment", durable: true, exclusive: false, autoDelete: false, arguments: null);
//        }
//        private void PublishMessage(Guid ticketId)
//        {
//            using (var connection = _factory.CreateConnection())
//            using (var channel = connection.CreateModel())
//            {
//                channel.QueueDeclare(queue: "ticket-booking",
//                                     durable: false,
//                                     exclusive: false,
//                                     autoDelete: false,
//                                     arguments: null);

//                var body = Encoding.UTF8.GetBytes(ticketId.ToString());

//                channel.BasicPublish(exchange: "",
//                                     routingKey: "ticket-booking",
//                                     basicProperties: null,
//                                     body: body);
//            }
//        }

//        public void PublishBatch(List<Guid> ticketIds)
//        {
//            var message = JsonConvert.SerializeObject(ticketIds);
//            var body = Encoding.UTF8.GetBytes(message);
//            _channel.BasicPublish(exchange: "", routingKey: "ticket_updates", basicProperties: null, body: body);
//        }
//        public async Task<List<Guid>> PublishBatchAndWaitForAcknowledgment(List<Guid> ticketIds)
//        {
//            var tcs = new TaskCompletionSource<List<Guid>>();

//            var acknowledgmentQueueName = "ticket_acknowledgment";
//            var consumer = new EventingBasicConsumer(_channel);

//            consumer.Received += (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var message = Encoding.UTF8.GetString(body);
//                var acknowledgedTicketIds = JsonConvert.DeserializeObject<List<Guid>>(message);
//                tcs.SetResult(acknowledgedTicketIds);
//            };

//            _channel.BasicConsume(queue: acknowledgmentQueueName, autoAck: true, consumer: consumer);

//            PublishBatch(ticketIds);

//            return await tcs.Task;
//        }
//    }
//}