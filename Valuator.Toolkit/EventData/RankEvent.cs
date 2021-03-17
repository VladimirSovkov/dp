using System;

namespace Valuator.Toolkit.EventData
{
    [Serializable]
    public class RankEvent
    {
        public string Id { get; set; }
        public double Rank { get; set; }
    }
}
