using System;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Hierarchies;

namespace LSRetail.Omni.Domain.Services.Base.Hierarchies
{
    public class HierarchyService
    {
        private IHierarchyRepository repository;

        public HierarchyService(IHierarchyRepository repository)
        {
            this.repository = repository;
        }

        public List<Hierarchy> GetHierarchies()
        {
            return repository.GetHierarchies();
        }
    }
}
