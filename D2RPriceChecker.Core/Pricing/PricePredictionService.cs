using D2RPriceChecker.Core.Traderie.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2RPriceChecker.Core.Pricing
{
    public class PricePredictionService
    {
        private RuneValueTable _runeValueTable;
        private readonly PriceConverter _priceConverter;
        private readonly SimilarityCalculator _similarityCalculator;
        private readonly RecencyWeighter _recencyWeighter;

        public PricePredictionService(RuneValueTable table)
        {
            _runeValueTable = table;
            _priceConverter = new PriceConverter(_runeValueTable);
            _similarityCalculator = new SimilarityCalculator();
            _recencyWeighter = new RecencyWeighter();
        }

        public PricePredictionResult Predict(List<string> itemText, List<Trade> trades)
        {
            var result = new PricePredictionResult();

            // Step 1: Compute the weighted sum and weight total
            double weightedSum = 0;
            double weightTotal = 0;

            foreach (var trade in trades)
            {
                double price = _priceConverter.Convert(trade);
                double similarity = _similarityCalculator.Compute(trade, itemText);
                double recency = _recencyWeighter.GetWeight(trade);

                // Weight calculation based on similarity and recency
                double weight = Math.Pow(similarity, 2) * recency; // You can tweak this formula

                weightedSum += price * weight;
                weightTotal += weight;
            }

            // Step 2: Calculate the final price prediction and confidence score
            result.Value = weightedSum / weightTotal; // The predicted price
            result.Confidence = weightTotal / (weightTotal + 2.0); // Confidence score (0-1 scale)

            result.WeightTotal = weightTotal;  // The total weight used in calculation
            result.TradeCount = trades.Count;  // Number of trades used in calculation

            return result;
        }
    }
}