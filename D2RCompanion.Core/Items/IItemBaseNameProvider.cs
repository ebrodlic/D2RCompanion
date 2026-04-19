using System;
using System.Collections.Generic;
using System.Text;

namespace D2RCompanion.Core.Items
{
    public interface IItemBaseNameProvider
    {
        IReadOnlyList<BaseNameEntry> GetAllBaseNames();
    }
}
