using D2RPriceChecker.Core.Traderie.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2RPriceChecker.Core.Traderie.Mapping
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
