using NATS.Client;
using System;
using System.Text.Json;
using Valuator.Toolkit;
using Valuator.Toolkit.EventData;

namespace EventsLogger
{
    public class EventsLogger
    {
        private IAsyncSubscription _subscription;
        private readonly IConnection _connection = new ConnectionFactory().CreateConnection();

        public EventsLogger()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _subscription = _connection.SubscribeAsync(Constants.RANK_CALCULATE_EVENT_NAME, (sender, args) =>
            {
                RankEvent rank = JsonSerializer.Deserialize<RankEvent>(args.Message.Data);
                Console.WriteLine($"Event: {args.Message.Subject}");
                Console.WriteLine($"Id: {rank.Id}");
                Console.WriteLine($"Rank: {rank.Rank}\n");
            });

            _subscription = _connection.SubscribeAsync(Constants.SIMILARITY_CALCULATE_EVENT_NAME, (sender, args) =>
            {
                SimilarityEvent similarity = JsonSerializer.Deserialize<SimilarityEvent>(args.Message.Data);
                Console.WriteLine($"Event: {args.Message.Subject}");
                Console.WriteLine($"Id: {similarity.Id}");
                Console.WriteLine($"Similarity: {similarity.Similarity}\n");
            });
        }

        public void Run()
        {
            _subscription.Start();
            
            Console.WriteLine("Press \"Enter\" to exit.\n");
            Console.ReadLine();

            _subscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();
        }
    }
}
