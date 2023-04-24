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
public class ListController : ControllerBase
{
    private readonly ListDBService _ListService;
    private readonly MembersDBService _membersSerivce;
    private readonly HomeDBService _homeDBService;
    private readonly TimeDBService _timeService;
    private readonly MailService _mailService;
    public ListController(ListDBService listDBService, MembersDBService membersDBService, HomeDBService homeDBService, TimeDBService timeDBService, MailService mailService)
    {
        _ListService = listDBService;
        _membersSerivce = membersDBService;
        _homeDBService = homeDBService;
        _timeService = timeDBService;
        _mailService = mailService;
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    #region 取得已預約時間清單
    [HttpGet]
    public IActionResult GetBookList()
    {
        string account = User.Identity.Name;
        if (User.Identity.Name == null)
        {
            return BadRequest("請去登入");
        }
        List<BookList> DataList = _ListService.GetBookTime(account);
        if (DataList == null)
        {
            return Ok("無預約資訊");
        }
        return Ok(DataList);
    }
    #endregion
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    #region 新增預約 有考量重疊部分了
    [HttpPost("AddBooking")]
    public IActionResult AddBooking(BookList Data)
    {
        Data.renter = User.Identity.Name;
        Members Members = _membersSerivce.GetDataByAccount(Data.renter);
        if (User.Identity.Name == null)
        {
            return BadRequest("請去登入");
        }
        var rental = _homeDBService.GetDataById(Data.rental_id);
        Data.publisher = rental.publisher;
        bool timeover = _ListService.IsTimeOverlap(Data.booktime, Data.bookdate, Data.renter);
        if (timeover == true)
        {
            bool booked = _ListService.CheckBooked(Data.renter, Data.bookdate, Data.booktime);
            return BadRequest("看房時間重疊！");
            if (booked == true)
            {
                return BadRequest("您已預約看房！");
            }
        }
        else
        {
            _ListService.Addbooking(Data);
        }
        //寄預約信
        string filePath = @"Views/BookMail.html";
        string TempString = System.IO.File.ReadAllText(filePath);
        string MailBody = _mailService.BookMailBody(TempString, User.Identity.Name, Data.bookdate, Data.booktime, rental.address);
        _mailService.SentBookMail(MailBody, Members.email);
        return Ok("新增預約成功");
    }
    #endregion

    [AllowAnonymous]
    #region 確認是否被預約 點選button要先確認->CheckBooking！這不需要了！

    [HttpGet("CheckBooking")]
    public IActionResult CheckBook([FromBody] CheckBooked Data)
    {
        // 檢查指定的時間是否已經被預約
        var isBooked = _ListService.IsBooked(Data.bookdate, Data.booktime, Data.publisher);

        if (isBooked)
        {
            return BadRequest("指定的時間已經被預約了");
        }
        else
        {
            return Ok("指定的時間可以預約");
        }
    }
    #endregion

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    #region 取消預約
    [HttpDelete("CancelBooking")]
    public IActionResult CancelBooking(Guid id)
    {
        try
        {
            _ListService.CancelBooking(id);
            return Ok("取消預約成功");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message.ToString());
        }
    }
    #endregion
}
