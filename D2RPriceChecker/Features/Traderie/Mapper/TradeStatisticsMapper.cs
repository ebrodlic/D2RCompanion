using D2RPriceChecker.Domain;
using D2RPriceChecker.Features.Traderie.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2RPriceChecker.Features.Traderie.Mapper
{
    public static class TradeStatisticsMapper
    {
        public static TradeStatistics Map(TradeStatisticsDto dto)
        {
            return new TradeStatistics
            {
                RuneValues = MapRunes(dto.RuneValues),
                Percentiles = dto.Percentiles,
                Average = dto.Average,
            };
        }

        private static List<RuneValue> MapRunes(Dictionary<string, double> runes)
        {
            return runes
                .Select(kvp => new RuneValue
                {
                    Name = kvp.Key,
                    ShortName = kvp.Key.Replace(" Rune", ""),
                    IstValue = kvp.Value
                })
                .OrderByDescending(x => x.IstValue)
                .ToList();
        }
    }
}
