using System;
using System.Collections.Generic;
using System.Text;

namespace D2RPriceChecker.Features.Traderie.Model
{
    public class ListingProperty
    {
        public string Type { get; set; } = "";
        public string Property { get; set; } = "";
        public int? Number { get; set; }
        public string? String { get; set; }
        public bool? Bool { get; set; }
    }
}
