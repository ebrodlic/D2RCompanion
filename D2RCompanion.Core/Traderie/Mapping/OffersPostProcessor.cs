using System;
using System.Collections.Generic;
using System.Text;
using D2RCompanion.Core.Traderie.Domain;

namespace D2RCompanion.Core.Traderie.Mapping
{
    public static class OffersPostProcessor
    {
        public static void Process(List<Trade> trades)
        {
            foreach (var trade in trades)
            {
                trade.PriceGroups = trade.Prices
                    .GroupBy(p => p.Group)
                    .Select(g => new PriceGroup
                    {
                        GroupId = g.Key,
                        Prices = g.ToList()
                    })
                    .ToList();
            }
        }
    }
}
