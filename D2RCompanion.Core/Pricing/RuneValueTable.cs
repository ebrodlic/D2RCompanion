using D2RCompanion.Core.Traderie.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Pricing
{
    public class RuneValueTable
    {
        private readonly Dictionary<string, RuneValue> _values;

        public  RuneValueTable(IEnumerable<RuneValue> values)
        {
            _values = values.ToDictionary(v => v.Name, v => v);
        }
        public bool HasValue(string name)
        {
            return _values.ContainsKey(name);
        }
        public double GetValue(string name)
        {
            return _values.TryGetValue(name, out var rune)
                ? rune.IstValue
                : 0;
        }
    }
}
