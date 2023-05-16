
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
                MethodInfo serviceMethod = typeof(AllDataDBService).GetMethod(methodName);
                object result = serviceMethod?.Invoke(_alldataSerivce, new object[] { day });

                PropertyInfo property = typeof(AllDayViewModel).GetProperty(day);
                if (property != null)
                {
                    property.SetValue(Data, result);
                }
            }
            return Ok(Data);
        }
    }

}

            /*Data.Mon=_alldataSerivce.Allmon();
            Data.Tue=_alldataSerivce.Alltue();
            Data.Wed=_alldataSerivce.Allwed();
            Data.Thu=_alldataSerivce.Allthu();
            Data.Fri=_alldataSerivce.Allfri();
            Data.Sat=_alldataSerivce.Allsat();
            Data.Sun=_alldataSerivce.Allsun();*/