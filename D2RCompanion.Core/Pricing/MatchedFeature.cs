using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Pricing
{
    public class MatchedFeature
    {
        public string FeatureKey { get; set; }
        public string OcrLine { get; set; }     // original text line
        public double Value { get; set; }
        public double Weight { get; set; }      // similarity * recency
    }
}
