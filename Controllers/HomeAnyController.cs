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
public class HomeAnyController : ControllerBase
{
    private readonly HomeDBService _homeDBService;
    private readonly HomeAnyDBService _homeanyDBService;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    private readonly IHttpContextAccessor _httpContextAccessor;
    public HomeAnyController(HomeDBService homeDBService,HomeAnyDBService homeanyDBService, IWebHostEnvironment env, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
    {
        _homeDBService = homeDBService;
        _homeanyDBService = homeanyDBService;
        _env = env;
        _configuration = configuration;
        _httpContextAccessor=httpContextAccessor;
    }

    //全部資料降冪
    [AllowAnonymous]
    [HttpGet("HomeAnyDownTime")]
    public IActionResult HomeAnyDownTime(int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeanyDBService.GetIdListDown(Data.Paging);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if (newBlock.AllData != null)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        return Ok(Data);
    }

    //全部資料升冪
    [AllowAnonymous]
    [HttpGet("HomeAnyUpTime")]
    public IActionResult HomeAnyUpTime(int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeanyDBService.GetIdListUp(Data.Paging);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if (newBlock.AllData != null)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        return Ok(Data);
    }

    //有搜尋值降冪
    [AllowAnonymous]
    [HttpPost("HomeAnySearchDown")]
    public IActionResult HomeAnySearchDown([FromForm]AnySearchViewModel SearchWord,int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.Search=new List<AnySearchViewModel>{SearchWord};
        Data.IdList = _homeanyDBService.GetIdListDown(Data.Paging,SearchWord);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if (newBlock.AllData != null)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if(Data.RentalBlock.Count==0)
        {
            return Ok("查無此資料");
        }
        return Ok(Data);
    }
    
    //有搜尋值升冪
    [AllowAnonymous]
    [HttpPost("HomeAnySearchUp")]
    public IActionResult HomeAnySearchUp([FromForm]AnySearchViewModel SearchWord,int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.Search=new List<AnySearchViewModel>{SearchWord};
        Data.IdList = _homeanyDBService.GetIdListUp(Data.Paging,SearchWord);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if (newBlock.AllData != null)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if(Data.RentalBlock.Count==0)
        {
            return Ok("查無此資料");
        }
        return Ok(Data);
    }

    //單筆資料
    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult AnyRead(Guid Id)
    {
        try
        {
            Rental Data = _homeDBService.GetDataById(Id);

            if (Data != null)
            {
                if(Data.isDelete==true || Data.tenant==false)
                {
                    return Ok("資訊已移除");
                }
                return Ok(Data);
            }
            return Ok("查詢單筆資料");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    //新增蒐藏
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    [HttpPost("AllCollect/{id}")]
    public IActionResult AllCollect([FromQuery]Collect Data,[FromQuery]Guid rental_id)
    {
        Data.renter=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        Data.rental_id=rental_id;
        _homeanyDBService.InsertCollect(Data);
        return Ok("已蒐藏");
    }
}