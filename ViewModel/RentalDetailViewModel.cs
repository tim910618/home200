using api1.ViewModel;
using api1.Service;
using api1.Models;

namespace api1.ViewModel
{
    public class RentaldetailViewModel
    {
        public Rental AllData { get; set; } 
        public bool IsCollected {get;set;}
        public string[] Image{get;set;} 
    }
}