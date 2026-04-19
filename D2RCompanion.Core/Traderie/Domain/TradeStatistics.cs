using D2RCompanion.Core.Traderie.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Traderie.Domain
{
    public class TradeStatistics
    {
        public List<RuneValue> RuneValues { get; set; } = new();
        public Percentiles Percentiles { get; set; } = new();
        public double Average {  get; set; }
    }
}
