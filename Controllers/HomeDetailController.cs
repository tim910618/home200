using Microsoft.AspNetCore.Mvc;
using api1.Service;
using api1.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using api1.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
namespace api1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeDetailController : ControllerBase
{
    private readonly HomeDBService _homeDBService;
    private readonly HomeDetailDBService _homedetailDBService;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    private readonly IHttpContextAccessor _httpContextAccessor;
    public HomeDetailController(HomeDBService homeDBService,HomeDetailDBService homedetailDBService, IWebHostEnvironment env, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
    {
        _homeDBService = homeDBService;
        _homedetailDBService=homedetailDBService;
        _env = env;
        _configuration = configuration;
        _httpContextAccessor=httpContextAccessor;
    }

    //尚未審核中全部資料
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpGet("AdminCheck")]
    public IActionResult AdminCheck(int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homedetailDBService.GetIdList(Data.Paging);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            // 宣告一個新陣列內物件
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if(newBlock.AllData.isDelete==false && newBlock.AllData.tenant == false)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if(Data.RentalBlock.Count==0)
        {
            return Ok("無資料");
        }
        return Ok(Data);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpGet("{id}")]
    public IActionResult AdminReadImg(Guid Id)
    {
        try
        {
            Rental data=_homeDBService.GetDataById(Id);
            if (data.isDelete==true || data==null)
            {
                return Ok("查無此資訊");
            }
            return Ok(data);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    //審核
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPut("Check")]
    public IActionResult Check([FromQuery]Guid Id,[FromQuery]int type)
    {
        var data = _homeDBService.GetDataById(Id);
        Rental CheckData=new Rental();
        if (data.isDelete==true || data==null)
        {
            return Ok("查無此資訊");
        }
        CheckData.rental_id = Id;
        CheckData.check=type;
        _homedetailDBService.CheckHome(CheckData);
        if(CheckData.check==1)
        {
            return Ok("審核通過");
        }
        else
        {
            return Ok("審核未通過");
        }
    }
}