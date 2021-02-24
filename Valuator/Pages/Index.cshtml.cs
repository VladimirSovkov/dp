using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            _storage.Load(similarityKey, CalculateSimilarity(text).ToString());

            string textKey = "TEXT-" + id;
            _storage.Load(textKey, text);

            string rankKey = "RANK-" + id;
            _storage.Load(rankKey, CalculateRank(text).ToString());

            return Redirect($"summary?id={id}");
        }

        public double CalculateRank(string text)
        {
            var countLetter = text.Where(x => !Char.IsLetter(x)).Count();
            return (double)countLetter / text.Count();
        }

        public double CalculateSimilarity(string text)
        {
            if (_storage.GetValues("TEXT-").Where(x => x == text).Count() > 0)
            {
                return (double)1.0;
            }
            return (double)0.0;
        }
    }
}
