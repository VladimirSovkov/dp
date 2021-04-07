using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Valuator.Toolkit;
using Valuator.Toolkit.Storage;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;
        private readonly int MaxWaitingTime = 100;
        private readonly TimeSpan DelayTime = TimeSpan.FromSeconds(0.01);

        public SummaryModel(ILogger<SummaryModel> logger,
            IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug($"LOOKUP: {id}, {_storage.GetShardKeyById(id)}.");
            Rank = GetRank(id);
            Similarity = Math.Round(Convert.ToDouble(_storage.GetValue(id, Constants.SIMILARITY_PREFIX + id)));
        }

        public double GetRank(string id)
        {
            int count = 0;
            string rankStr;
            while (count < MaxWaitingTime)
            {
                rankStr = _storage.GetValue(id, Constants.RANK_PREFIX + id);
                if (!String.IsNullOrWhiteSpace(rankStr))
                {
                    return Math.Round(Convert.ToDouble(rankStr), 2);
                }
                else
                {
                    Thread.Sleep(DelayTime);
                    count++;
                }
            }
            return 0.0;
        }
    }
}
