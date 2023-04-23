using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using api1.ViewModel;
using api1.Service;
using api1.Secruity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api1.Models;
using System.Web;

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    private readonly TimeDBService _timeService;
    private readonly MembersDBService _MemberService;

    public TimeController(TimeDBService timeDBService, MembersDBService membersDBService)
    {
        _timeService = timeDBService;
        _MemberService = membersDBService;
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    #region 房東設定可預約時間
    [HttpPost("SetBookTime")]
    public IActionResult SetBookTime([FromBody] BookTime Data)
    {
        Data.publisher = User.Identity.Name;
        if (User.Identity.Name == null)
        {
            return BadRequest("請去登入");
        }
        Data.booktime_id = _timeService.GetBookTime_Id(Data.publisher);

        _timeService.SetBookTime(Data.publisher, Data);
        return Ok("時間設定成功");
    }
    #endregion
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    #region 房東取得可預約時間表
    [HttpGet("BookTime")]
    public IActionResult BookTimeData()
    {
        string account = User.Identity.Name;
        if (User.Identity.Name == null)
        {
            return BadRequest("請去登入");
        }
        BookTime Data = _timeService.GetBookTime(account);
        return Ok(Data);
    }
    #endregion
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    #region 設定特別單一時段
    [HttpPost("SetSpecialTime")]
    public IActionResult AddSpecialTime([FromBody] SpecialTime Data)
    {
        // 檢查使用者是否有權限新增 SpecialTime 資料
        Data.publisher = User.Identity.Name;
        if (User.Identity.Name == null)
        {
            return BadRequest("請去登入");
        }
        // 取得原本的 BookTime 資料
        BookTime bookTime = _timeService.GetBookTime(Data.publisher);
        Data.special_id = _timeService.GetSpecialTime_Id(Data.publisher);
        // 根據日期取得原本的可預約時間
        string oldTime;
        switch (Data.date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                oldTime = bookTime.mon;
                break;
            case DayOfWeek.Tuesday:
                oldTime = bookTime.tue;
                break;
            case DayOfWeek.Wednesday:
                oldTime = bookTime.wed;
                break;
            case DayOfWeek.Thursday:
                oldTime = bookTime.thu;
                break;
            case DayOfWeek.Friday:
                oldTime = bookTime.fri;
                break;
            case DayOfWeek.Saturday:
                oldTime = bookTime.sat;
                break;
            case DayOfWeek.Sunday:
                oldTime = bookTime.sun;
                break;
            default:
                throw new ArgumentException("請輸入正確日期");
        }
        Data.oldtime = oldTime;
        if (!string.IsNullOrEmpty(oldTime))
        {
            // 拆分原本的可預約時間，找出要修改的區間
            List<string> timeList = oldTime.Split(';').ToList();
            string oldInterval = timeList.Find(i => i == Data.oldtime);

            timeList.Remove(oldInterval);
            timeList.Add(Data.newtime);

            // 將修改後的可預約時間組合成字串，更新到 BookTime 資料表中
            string newTime = string.Join(";", timeList);
            switch (Data.date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    bookTime.mon = newTime;
                    break;
                case DayOfWeek.Tuesday:
                    bookTime.tue = newTime;
                    break;
                case DayOfWeek.Wednesday:
                    bookTime.wed = newTime;
                    break;
                case DayOfWeek.Thursday:
                    bookTime.thu = newTime;
                    break;
                case DayOfWeek.Friday:
                    bookTime.fri = newTime;
                    break;
                case DayOfWeek.Saturday:
                    bookTime.sat = newTime;
                    break;
                case DayOfWeek.Sunday:
                    bookTime.sun = newTime;
                    break;
                default:
                    throw new ArgumentException("請輸入正確日期");
            }
        }
        // 新增 SpecialTime 資料
        _timeService.AddSpecialTime(Data);

        return Ok("新增單一時段成功");
    }
    #endregion
    [AllowAnonymous]
    #region 取得單天的預約時間 RentalController用吧
    [HttpGet("BookOfDay/{date}")]
    public IActionResult BookOfDay(DateTime date)
    {
        // 先取得 Rental 的屋主是誰，這裡先用 admin 
        string account = "admin";
        // bookTime.publisher=User.Identity.Name;
        // if(User.Identity.Name==null){
        //     return BadRequest("請去登入");
        // }
        // 先查詢 SpecialTime 表格中有沒有指定日期的資料
        SpecialTime specialTime = _timeService.GetSpecialTime(account, date);
        string availableTimes;

        if (specialTime != null)
        {
            // 如果有資料，就以該筆資料的可預約時段為主
            availableTimes = specialTime.newtime;
        }
        else
        {
            // 如果沒有資料，就從 BookTime 中取得當天的可預約時段
            BookTime bookTime = _timeService.GetBookOfDay(account);
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    availableTimes = bookTime.mon;
                    break;
                case DayOfWeek.Tuesday:
                    availableTimes = bookTime.tue;
                    break;
                case DayOfWeek.Wednesday:
                    availableTimes = bookTime.wed;
                    break;
                case DayOfWeek.Thursday:
                    availableTimes = bookTime.thu;
                    break;
                case DayOfWeek.Friday:
                    availableTimes = bookTime.fri;
                    break;
                case DayOfWeek.Saturday:
                    availableTimes = bookTime.sat;
                    break;
                case DayOfWeek.Sunday:
                    availableTimes = bookTime.sun;
                    break;
                default:
                    throw new ArgumentException("請選擇正確的日期格式");
            }
        }

        // 將已被預約的時段轉成陣列
        //string[] bookedTimes = _timeService.GetBookedTimes(account, date);
        // 將當天所有可預約的時段轉成陣列
        string[] availableTimesArray = availableTimes.Split(';');
        // 取得未被預約的時段
        //string[] unbookedTimes = availableTimesArray.Except(bookedTimes).ToArray();

        //return Ok(new { bookedTimes, unbookedTimes });
        return Ok(new { availableTimesArray });
    }

    #endregion
}
