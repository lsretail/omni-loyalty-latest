
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.Services.Loyalty.Search;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Setup
{
    public class SearchRepository : BaseRepository, ISearchRepository
    {
        public SearchRs Search(string contactId, string search, int maxResultset, SearchType searchTypes)
        {
            string methodName = "Search";
            var jObject = new { contactId = contactId, search = search, searchTypes = (int)searchTypes };
            return base.PostData<SearchRs>(jObject, methodName);
        }
    }
}
