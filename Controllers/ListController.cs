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
        if (!User.Identity.IsAuthenticated)
        {
            return BadRequest("請登入");
        }
        string account = User.Identity.Name;
        List<GetBookListViewModel> DataList = _ListService.GetBookTime(account);
        if (DataList == null)
        {
            return BadRequest("無預約資訊");
        }
        return Ok(DataList);
    }
    #endregion
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    #region 新增預約 有考量重疊部分了
    [HttpPost("AddBooking")]
    public IActionResult AddBooking(BookList Data)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Ok("請登入");
        }
        Data.renter = User.Identity.Name;
        Members Members = _membersSerivce.GetDataByAccount(Data.renter);
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
        }
        else if (otherbooked == true)
        {
            return BadRequest("已被其他房客預約！");
        }
        else
        {
            //無法預約今天(含)以前的判斷
            if (DateTime.Compare(Data.bookdate.ToDateTime(new TimeOnly()), DateTime.Now) < 0)
            {
                return BadRequest("超過預約期間，不可預約");
            }
            _ListService.Addbooking(Data);
        }
        //寄預約信
        string filePath = @"Views/BookMail.html";
        string TempString = System.IO.File.ReadAllText(filePath);
        // string MailBody = _mailService.BookMailBody(TempString, publisher.name, Data.bookdate, Data.booktime, rental.address);
        // _mailService.SentBookMail(MailBody, publisher.email);
        return Ok(Data);
    }
    #endregion

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    #region 取消預約
    [HttpPost("CancelBooking")]
    public IActionResult CancelBooking([FromBody] CancelBooking BookList)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return BadRequest("請登入");
        }
        //取得Booklist、取得房東房客的Email
        BookList Data = new BookList();
        Data = _ListService.GetBookTimeById(BookList.BookList_id);
        Rental rental = new Rental();
        rental = _homeDBService.GetDataById(Data.rental_id);
        if (Data == null || Data.isDelete == true)
        {
            return BadRequest("查無預約資料");
        }
        else
        {
            Members publisher = _membersSerivce.GetDataByAccount(Data.publisher);
            Members renter = _membersSerivce.GetDataByAccount(Data.renter);
            string filePath = @"Views/CancelBooking.html";
            string TempStringP = System.IO.File.ReadAllText(filePath);
            string MailBody = _mailService.CancelMailBody(TempStringP, publisher.name, Data.bookdate, Data.booktime, rental.title);
            _mailService.SentBookMail(MailBody, publisher.email);
            string TempStringR = System.IO.File.ReadAllText(filePath);
            string MailBodyR = _mailService.CancelMailBody(TempStringR, renter.name, Data.bookdate, Data.booktime, rental.title);
            _mailService.SentBookMail(MailBodyR, renter.email);
            _ListService.CancelBooking(BookList.BookList_id);
        }
        return Ok("取消預約成功");
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////
    #region 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("SentBookingCheck")]
    public IActionResult SentBookingCheck(BookList Data)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Ok("請登入");
        }
        Data.renter = User.Identity.Name;
        Members Members = _membersSerivce.GetDataByAccount(Data.renter);
        var rental = _homeDBService.GetDataById(Data.rental_id);
        Data.publisher = rental.publisher;
        Members publisher = _membersSerivce.GetDataByAccount(Data.publisher);
        bool otherbooked = _ListService.IsBooked(Data.bookdate, Data.booktime, Data.publisher, Data.renter);
        if (otherbooked == true)
        {
            return Ok("已被其他房客預約！");
        }
        bool timeover = _ListService.IsTimeOverlap(Data.booktime, Data.bookdate, Data.renter);
        if (timeover == true)
        {
            return Ok("看房時間重疊！");
        }
        bool booked = _ListService.CheckBooked(Data.renter, rental.publisher, Data.bookdate, Data.booktime);
        if (booked == true)
        {
            return Ok("您已預約看房！");
        }
        else
        {
            //無法預約今天(含)以前的判斷
            if (DateTime.Compare(Data.bookdate.ToDateTime(new TimeOnly()), DateTime.Now) < 0)
            {
                return Ok("超過預約期間，不可預約");
            }
            _ListService.Addbooking(Data);
            Guid? Book_Id = Data.booklist_id;
            //寄確認預約信
            string filePath = @"Views/BookMail.html";
            string TempString = System.IO.File.ReadAllText(filePath);
            string MailBody = _mailService.BookMailBody(TempString, publisher.name, Data.bookdate, Data.booktime, rental.address, Book_Id);
            _mailService.SentBookMail(MailBody, publisher.email);
            return Ok(Data);
        }
    }
    #endregion

    //還要在信寄信通知
    [AllowAnonymous]
    [HttpPost("CheckBooking")]
    public IActionResult CheckBooking([FromBody]CheckBookingViewModel Data)
    {
        string CheckString= _ListService.CheckBooking(Data.Book_Id,Data.state);
        return Ok(CheckString);
    }
}
