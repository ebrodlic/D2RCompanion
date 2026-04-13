using D2RPriceChecker.Features.Traderie.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2RPriceChecker.Domain
{
    public class TradeStatistics
    {
        public List<RuneValue> RuneValues { get; set; } = new();
        public Percentiles Percentiles { get; set; } = new();
        public double Average {  get; set; }
    }
}
