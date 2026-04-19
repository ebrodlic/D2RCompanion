using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Traderie.DTO
{
    public class TradeStatisticsDto
    {
        public int TotalTrades { get; set; }
        public int ValuedTrades { get; set; }
        public int ExcludedTrades { get; set; }
        public double Average { get; set; }
        public Percentiles Percentiles { get; set; } = new();
        public List<DistributionEntry> Distribution { get; set; } = new();
        public Dictionary<string, double> RuneValues { get; set; } = new();
        public DateRange DateRange { get; set; } = new();
        public string Version { get; set; } = "";
    }

    public class Percentiles
    {
        public double Floor { get; set; }
        public double Typical { get; set; }
        public double Good { get; set; }
        public double High { get; set; }
    }

    public class DistributionEntry
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public double IstValue { get; set; }
        public int Percentage { get; set; }
    }

    public class DateRange
    {
        public DateTime Oldest { get; set; }
        public DateTime Newest { get; set; }
    }
}
