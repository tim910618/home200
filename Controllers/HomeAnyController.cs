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

    //某個房東全部資料
    [AllowAnonymous]
    [HttpGet("HomeAnySeePublisher")]
    public IActionResult HomeAnySeePublisher([FromQuery]string publisher,int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeanyDBService.GetIdListSeePublisher(Data.Paging,publisher);
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
            return Ok("此房東無上架房屋");
        }
        return Ok(Data);
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
        if(Data.RentalBlock.Count==0)
        {
            return Ok("無資料");
        }
        return Ok(Data);
    }

    [AllowAnonymous]
    [HttpGet("HomeAnyDownTime1")]
    public IActionResult HomeAnyDownTime1(int Page = 1)
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
        if(Data.RentalBlock.Count==0)
        {
            return Ok("無資料");
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
        if(Data.RentalBlock.Count==0)
        {
            return Ok("無資料");
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
            else
            {
                return Ok("查無此資料");
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }



    //蒐藏全部資料
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    [HttpPost("AllCollect")]
    public IActionResult AllCollect(int Page=1)
    {
        Collect collect=new Collect();
        collect.renter=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        AllCollectListViewModel ViewData = new AllCollectListViewModel();
        ViewData.Paging = new ForPaging(Page);
        ViewData.IdList = _homeanyDBService.GetIdListAllCollect(ViewData.Paging,collect.renter);
        ViewData.RentalBlock = new List<AllCollectViewModel>();
        foreach (var Id in ViewData.IdList)
        {
            AllCollectViewModel newBlock = new AllCollectViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if (newBlock.AllData != null)
            {
                newBlock.isDelete=false;
                ViewData.RentalBlock.Add(newBlock);
            }
        }
        if(ViewData.RentalBlock.Count==0)
        {
            return Ok("無資料");
        }
        return Ok(ViewData);
    }
    //新增蒐藏
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    [HttpPost("AddCollect")]
    public IActionResult AddCollect([FromQuery]Collect Data,Guid rental_id)
    {
        Data.renter=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        bool CheckCollect=_homeanyDBService.CheckCollect(Data.renter,rental_id);
        if(CheckCollect==true)
        {
            return Ok("此房屋已經蒐藏過了");
        }
        _homeanyDBService.InsertCollect(Data);
        return Ok("蒐藏成功");
    }
    //取消蒐藏
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "renter")]
    [HttpDelete("RemoveCollect")]
    public IActionResult  RemoveCollect([FromQuery]Guid rental_id)
    {
        string renter=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        bool CheckCollect=_homeanyDBService.CheckCollect(renter,rental_id);
        if(CheckCollect==false)
        {
            return Ok("查無此蒐藏");
        }
        _homeanyDBService.RemoveCollect(renter,rental_id);
        return Ok("取消蒐藏");
    }
}


