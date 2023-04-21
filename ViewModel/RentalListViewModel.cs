using api1.ViewModel;
using api1.Service;

namespace api1.ViewModel
{
    public class RentalListViewModel
    {
        //搜尋
        public List<AnySearchViewModel>? Search { get; set; }
        public List<Guid> IdList{get;set;}
        public List<RentaldetailViewModel> RentalBlock { get; set; }
        public ForPaging Paging { get; set; }
    }
}