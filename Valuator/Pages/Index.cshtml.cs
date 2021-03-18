using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Valuator.Storage;

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

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            if (String.IsNullOrWhiteSpace(text))
            {
                return Redirect($"summary");
            }

            string id = Guid.NewGuid().ToString();

            string similarityKey = Constants.SIMILARITY_PREFIX + id;
            int simularity = 0;
            if (_storage.GetValues(Constants.TEXT_PREFIX).Where(x => x == text).Count() > 0)
            {
                simularity = 1;
            }
            _storage.Load(similarityKey, simularity.ToString());

            string textKey = Constants.TEXT_PREFIX + id;
            _storage.Load(textKey, text);

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => CalculateAndSaveRank(id), cts.Token);

            return Redirect($"summary?id={id}");
        }

        private async Task CalculateAndSaveRank(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish(Constants.RANK_CALCULATE_EVENT_NAME, data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
    }
}
