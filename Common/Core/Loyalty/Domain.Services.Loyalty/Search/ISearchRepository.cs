using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Search
{
    public interface ISearchRepository
    {
        SearchRs Search(string contactId, string search, int maxResultset, SearchType searchType);
    }
}
