using D2RCompanion.Core.Traderie.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace D2RCompanion.Core.Pricing
{
    public class SimilarityCalculator
    {
        public double Compute(Trade trade, List<string> itemText)
        {
            var tradeFeatures = ExtractFeatures(trade.Properties);
            var knownFeatures = tradeFeatures.Keys;

            var ocrFeatures = ExtractFeatures(itemText, knownFeatures);

            return Compare(tradeFeatures, ocrFeatures);
        }

        // Extract features from trade properties (like from a Trade object)
        public Dictionary<string, double> ExtractFeatures(IEnumerable<ListingProperty> props)
        {
            var features = new Dictionary<string, double>();

            foreach (var prop in props)
            {
                if (!IsRelevant(prop)) continue;

                var key = prop.Property.Trim();
                var value = prop.Number!.Value;

                features[key] = value;
            }

            return features;
        }

        // Extract features from OCR text (matching with known features)
        public Dictionary<string, double> ExtractFeatures(IEnumerable<string> ocrLines, IEnumerable<string> knownFeatures)
        {
            var result = new Dictionary<string, double>();

            foreach (var line in ocrLines)
            {
                var value = ParseNumeric(line);
                if (value == null) continue;

                var match = MatchFeature(line, knownFeatures);
                if (match == null) continue;

                result[match] = value.Value;
            }

            return result;
        }

        // Match OCR lines with known features using a fuzzy approach
        public List<string> MatchFeaturesWithOcrText(List<string> ocrLines, List<string> featureKeys)
        {
            var matchedFeatures = new List<string>();

            foreach (var line in ocrLines)
            {
                var matchedKey = MatchFeatureWithThreshold(line, featureKeys);
                if (matchedKey != null)
                {
                    matchedFeatures.Add(matchedKey);
                }
            }

            return matchedFeatures;
        }

        private string? MatchFeatureWithThreshold(string line, List<string> featureKeys)
        {
            string normalizedLine = Normalize(line);

            foreach (var key in featureKeys)
            {
                string normalizedKey = Normalize(key);

                if (normalizedLine.Contains(normalizedKey))
                {
                    return key;
                }
            }

            return null;
        }

        // More aggressive normalization to clean up the text for matching
        private string Normalize(string text)
        {
            return Regex.Replace(text, @"\d+", "")            // Remove digits
                        .Replace("{value}", "")              // Remove {value} placeholder
                        .Replace("+", "")                    // Remove plus signs
                        .Replace("to", "")                   // Remove "to"
                        .ToLowerInvariant()                  // Convert to lowercase
                        .Replace("  ", " ")                  // Replace double spaces with single
                        .Trim();                             // Trim leading/trailing spaces
        }

        // Compare two sets of features for similarity
        private double Compare(Dictionary<string, double> a, Dictionary<string, double> b)
        {
            var keys = a.Keys.Union(b.Keys);
            double sum = 0;
            int count = 0;

            foreach (var key in keys)
            {
                var va = a.TryGetValue(key, out var av) ? av : 0;
                var vb = b.TryGetValue(key, out var bv) ? bv : 0;

                var diff = Math.Abs(va - vb);
                var norm = Math.Max(Math.Abs(va), Math.Abs(vb));

                if (norm > 0)
                    diff /= norm;

                sum += diff;
                count++;
            }

            return count == 0 ? 0 : 1.0 - (sum / count);
        }

        private double? ParseNumeric(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            var match = Regex.Match(line, @"([+-]?\d+(\.\d+)?)");

            if (!match.Success) return null;

            if (!double.TryParse(match.Groups[1].Value, out var value)) return null;

            return value;
        }

        private string? MatchFeature(string line, IEnumerable<string> knownFeatures)
        {
            var normalizedLine = Normalize(line);

            string? best = null;
            double bestScore = 0;

            foreach (var feature in knownFeatures)
            {
                var normalizedFeature = Normalize(feature);

                double score = FuzzyScore(normalizedLine, normalizedFeature);

                if (score > bestScore)
                {
                    bestScore = score;
                    best = feature;
                }
            }

            return bestScore > 0.5 ? best : null;
        }

        private double FuzzyScore(string a, string b)
        {
            if (a.Contains(b) || b.Contains(a)) return 0.9;

            var aTokens = a.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var bTokens = b.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var setA = aTokens.ToHashSet();
            var setB = bTokens.ToHashSet();

            var intersection = setA.Intersect(setB).Count();
            var union = setA.Union(setB).Count();

            return union == 0 ? 0 : (double)intersection / union;
        }

        private bool IsRelevant(ListingProperty p)
        {
            return p.Type == "number" && p.Number.HasValue;
        }
    }
}