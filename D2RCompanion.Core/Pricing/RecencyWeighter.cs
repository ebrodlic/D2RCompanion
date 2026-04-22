using System;
using System.Collections.Generic;
using System.Text;
using D2RCompanion.Core.Traderie.Domain;

namespace D2RCompanion.Core.Pricing
{
    public class RecencyWeighter
    {
        public double GetWeight(Trade trade)
        {
            var days = (DateTime.UtcNow - trade.UpdatedAt).TotalDays;

            return Math.Exp(-0.15 * days);
        }
    }
}
