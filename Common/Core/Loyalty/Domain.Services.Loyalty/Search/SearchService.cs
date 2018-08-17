using System.Linq;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace LSRetail.Omni.Domain.Services.Loyalty.Search
{
    public class SearchService
    {
        private ISearchRepository iSearchRepository;

        public SearchService(ISearchRepository iRepo)
        {
            iSearchRepository = iRepo;
        }

        public SearchRs Search(string contactId, string search, int maxResultset, SearchType searchType)
        {
            var res = iSearchRepository.Search(contactId, search, maxResultset, searchType);
            res.Items = res.Items.Where(x => x.AllowedToSell).ToList();
            return res;
        }

        public async Task<SearchRs> SearchAsync( string contactId, string search, int maxResultset, SearchType searchType )
        {
            return await Task.Run(() => Search(contactId, search, maxResultset, searchType));
        }
    }
}