using NATS.Client;
using System;
using System.Linq;
using System.Text;
using Valuator;
using Valuator.Storage;

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
            _subscription = _connection.SubscribeAsync(Constants.RANK_CALCULATE_EVENT_NAME, "rank-calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                var text = _storage.GetValue(Constants.TEXT_PREFIX + id);
                string rankKey = Constants.RANK_PREFIX + id;
                var rank = CalculateRank(text);
                _storage.Load(rankKey, rank.ToString());
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
    }
}
