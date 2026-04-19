using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Items
{
    public class BaseNameEntry
    {
        public string Original { get; set; } = "";
        public string Normalized { get; set; } = "";
        public string[] Tokens { get; set; } = [];
    }
}
