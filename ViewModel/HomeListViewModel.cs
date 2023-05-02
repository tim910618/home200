using api1.ViewModel;
using api1.Service;

namespace api1.ViewModel
{
    public class HomeListViewModel
    {
        //搜尋
        public List<Guid> IdList{get;set;}
        public List<HomeViewModel> RentalBlock { get; set; }
    }
}