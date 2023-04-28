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
        List<GetBookListViewModel> DataList = _ListService.GetBookTime(account);
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
        Members publisher = _membersSerivce.GetDataByAccount(Data.publisher);
        bool otherbooked = _ListService.IsBooked(Data.bookdate, Data.booktime, Data.publisher, Data.renter);
        bool timeover = _ListService.IsTimeOverlap(Data.booktime, Data.bookdate, Data.renter);
        bool booked = _ListService.CheckBooked(Data.renter, rental.publisher, Data.bookdate, Data.booktime);
        if (timeover == true)
        {
            return BadRequest("看房時間重疊！");
        }
        else if (booked == true)
        {
            return BadRequest("您已預約看房！");
        }else if(otherbooked==true)
        {
            return BadRequest("已被其他房客預約！");
        }
        else
        {
            _ListService.Addbooking(Data);
        }
        //寄預約信
        string filePath = @"Views/BookMail.html";
        string TempString = System.IO.File.ReadAllText(filePath);
        string MailBody = _mailService.BookMailBody(TempString, publisher.name, Data.bookdate, Data.booktime, rental.address);
        _mailService.SentBookMail(MailBody, publisher.email);
        return Ok(Data);
    }
    #endregion

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    #region 取消預約
    [HttpPost("CancelBooking")]
    public IActionResult CancelBooking([FromBody] CancelBooking Data)
    {
        _ListService.CancelBooking(Data.BookList_id);
        return Ok("取消預約成功");
    }
    #endregion
}
