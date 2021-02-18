using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            int simularity = 0;
            if (_storage.GetValues("TEXT-").Where(x => x == text).Count() > 0)
            {
                simularity = 1;
            }
            _storage.Load(similarityKey, simularity.ToString());

            string textKey = "TEXT-" + id;
            _storage.Load(textKey, text);

            string rankKey = "RANK-" + id;
            var countLetter = text.Where(x => Char.IsLetter(x)).Count();
            double rank = (double) countLetter / text.Count();
            _storage.Load(rankKey, rank.ToString());

            return Redirect($"summary?id={id}");
        }
    }
}
