using api1.ViewModel;
using api1.Service;

namespace api1.ViewModel
{
    public class RentalListViewModel
    {
        public List<Guid> IdList{get;set;}
        public List<RentaldetailViewModel> RentalBlock { get; set; }
        public ForPaging Paging { get; set; }
    }
}