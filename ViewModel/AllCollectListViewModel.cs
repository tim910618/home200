using api1.ViewModel;
using api1.Service;

namespace api1.ViewModel
{
    public class AllCollectListViewModel
    {
        //搜尋
        public List<AnySearchViewModel>? Search { get; set; }
        public List<Guid> IdList{get;set;}
        public List<AllCollectViewModel> RentalBlock { get; set; }
        public ForPaging Paging { get; set; }
    }
}