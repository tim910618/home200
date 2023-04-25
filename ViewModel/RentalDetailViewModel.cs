using api1.ViewModel;
using api1.Service;
using api1.Models;

namespace api1.ViewModel
{
    public class RentaldetailViewModel
    {
        public Rental AllData { get; set; } 
        public string ImagePath { get; set; } // 新增一個欄位用來存放圖片的絕對路徑
    }
}