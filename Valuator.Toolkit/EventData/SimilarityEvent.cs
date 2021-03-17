using System;

namespace Valuator.Toolkit.EventData
{
    [Serializable]
    public class SimilarityEvent
    {
        public string Id { get; set; }
        public double Similarity { get; set; }
    }
}
