
using System.Reflection;
using api1.Service;
using api1.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api1.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AllDataController : ControllerBase
    {
        private readonly AllDataDBService _alldataSerivce;
        public AllDataController(AllDataDBService alldataSerivce)
        {
            _alldataSerivce = alldataSerivce;
        }

        /*房東*/
        //型態
        [AllowAnonymous]
        [HttpGet("AllDataHomegenre")]
        public IActionResult AllDataHomegenre()
        {
            Dictionary<string, int> Data=_alldataSerivce.AllHomegenre();
            return Ok(Data);
        }

        //類型
        [AllowAnonymous]
        [HttpGet("AllDataHometype")]
        public IActionResult AllDataHometype()
        {
            Dictionary<string,int> Data=_alldataSerivce.AllHometype();
            return Ok(Data);
        }

        //地區
        [AllowAnonymous]
        [HttpGet("AllDataHomeaddress")]
        public IActionResult AllDataHomeaddress()
        {
            Dictionary<string,int> Data=_alldataSerivce.AllHomeaddress();
            return Ok(Data);
        }


        /*檢舉*/
        [AllowAnonymous]
        [HttpGet("AllDataReason")]
        public IActionResult AllDataReason()
        {
            List<string> Data=_alldataSerivce.AllReason();
            return Ok(Data);
        }

        /*房東時間*/
        [AllowAnonymous]
        [HttpGet("AllDataDay")]
        public IActionResult AllDataDay()
        {
            AllDayViewModel Data=new AllDayViewModel();
            List<string> daysOfWeek = new List<string> { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
            foreach (string day in daysOfWeek)
            {
                string methodName = $"All{day.ToLower()}";
                //這行程式碼使用反射獲取 AllDataDBService 類型中的方法，方法名稱為 methodName。該方法用於從資料庫中檢索特定星期幾的資料。
                MethodInfo serviceMethod = typeof(AllDataDBService).GetMethod(methodName);
                //這行程式碼調用上述獲取的方法，將 _alldataSerivce 物件作為目標物件，並將 day 作為方法的參數傳遞。?避免空值。
                //.Invoke 方法用於動態調用方法。它接受兩個參數：目標物件和方法的參數陣列。
                object result = serviceMethod?.Invoke(_alldataSerivce.Allday, new object[] { day });

                //這行程式碼使用反射獲取 AllDayViewModel 類型中的屬性，屬性名稱與 day 相符。該屬性用於設定星期幾的資料。
                PropertyInfo property = typeof(AllDayViewModel).GetProperty(day);
                if (property != null)
                {
                    //這行程式碼將 result 的值設定給 Data 物件的對應屬性，使用 property.SetValue 方法。
                    property.SetValue(Data, result);
                }
            }
            return Ok(Data);
        }
        /*簡單來說就是鎖定AllDataDBService，
        然後選擇_alldataSerivce.Allday再帶值day給_alldataSerivce.Allday*/
    }

}

            /*Data.Mon=_alldataSerivce.Allmon();
            Data.Tue=_alldataSerivce.Alltue();
            Data.Wed=_alldataSerivce.Allwed();
            Data.Thu=_alldataSerivce.Allthu();
            Data.Fri=_alldataSerivce.Allfri();
            Data.Sat=_alldataSerivce.Allsat();
            Data.Sun=_alldataSerivce.Allsun();*/