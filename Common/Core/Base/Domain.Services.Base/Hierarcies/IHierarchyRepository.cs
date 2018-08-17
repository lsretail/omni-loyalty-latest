using System;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Hierarchies;

namespace LSRetail.Omni.Domain.Services.Base.Hierarchies
{
    public interface IHierarchyRepository
    {
        List<Hierarchy> GetHierarchies();
        Hierarchy GetHierarchy(string id);
    }
}
