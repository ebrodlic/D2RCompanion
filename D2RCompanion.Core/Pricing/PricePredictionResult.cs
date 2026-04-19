using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Pricing
{
    public class PricePredictionResult
    {
        public double Value { get; set; }          // final predicted price
        public double Confidence { get; set; }     // 0–1
        public double WeightTotal { get; set; }    // how much support exists
        public int TradeCount { get; set; }        // raw sample size

        public List<MatchedFeature> MatchedFeatures { get; set; } = new(); // for future use: highlighting matched attributes in description
    }
}
