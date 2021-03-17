using NATS.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Valuator.Storage;
using Valuator.Toolkit;
using Valuator.Toolkit.EventData;

namespace RankCalculator
{
    public class RankCalculator
    {
        private readonly IStorage _storage;
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _subscription;

        public RankCalculator(IStorage storage)
        {
            _storage = storage;
            ConnectionFactory cf = new ConnectionFactory();
            _connection = cf.CreateConnection();
            _subscription = _connection.SubscribeAsync(Constants.RANK_CALCULATE, "rank-calculator", async (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                var text = _storage.GetValue(Constants.TEXT_PREFIX + id);
                string rankKey = Constants.RANK_PREFIX + id;
                var rank = CalculateRank(text);
                _storage.Load(rankKey, rank.ToString());

                await PublishRankCalculateEvent(id, Math.Round(rank, 2));
            });
        }

        public void Run()
        {
            _subscription.Start();

            Console.WriteLine("Press \"Enter\" to exit.");
            Console.ReadLine();

            _subscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();
        }

        public double CalculateRank(string text)
        {
            var countLetter = text.Where(x => !Char.IsLetter(x)).Count();
            return (double)countLetter / text.Count();
        }

        private async Task PublishRankCalculateEvent(string id, double rank)
        {
            RankEvent rankEvent = new RankEvent()
            {
                Id = id,
                Rank = rank
            };

            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rankEvent));
                c.Publish(Constants.RANK_CALCULATE_EVENT_NAME, data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
    }
}
