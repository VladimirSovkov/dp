using Microsoft.Extensions.Logging;
using Valuator.Storage;

namespace RankCalculator
{
    class Program
    {
        static void Main()
        {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            IStorage storage = new RedisStorage();
            var rankCalculator = new RankCalculator(storage, loggerFactory.CreateLogger<RankCalculator>());
            rankCalculator.Run();
        }
    }
}
