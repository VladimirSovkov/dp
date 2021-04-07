using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Valuator.Toolkit;
using Valuator.Toolkit.EventData;
using Valuator.Toolkit.Storage;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, 
            IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string countrySegment)
        {
            _logger.LogDebug(text);

            if (String.IsNullOrWhiteSpace(text))
            {
                return Redirect($"summary");
            }

            string id = Guid.NewGuid().ToString();

            _storage.AddShardKeyById(id, countrySegment);
            
            string similarityKey = Constants.SIMILARITY_PREFIX + id;
            int simularity = 0;
            if (_storage.IsContainsText(text))
            {
                simularity = 1;
            }
            
            _storage.AddValueByKey(id, similarityKey, simularity.ToString());
            PublishSimilarityCalculateEvent(id, simularity);

            string textKey = Constants.TEXT_PREFIX + id;
            _storage.AddValueByKey(id, textKey, text);
            _storage.AddTextToSet(id, text);

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => CalculateAndSaveRank(id), cts.Token);

            return Redirect($"summary?id={id}");
        }

        private void PublishSimilarityCalculateEvent(string id, int similarity)
        {
            SimilarityEvent similarityEvent = new SimilarityEvent
            {
                Id = id,
                Similarity = similarity
            };

            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(similarityEvent));
                c.Publish(Constants.SIMILARITY_CALCULATE_EVENT_NAME, data);

                c.Drain();
                c.Close();
            }
        }

        private async Task CalculateAndSaveRank(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish(Constants.RANK_CALCULATE, data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
    }
}
