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

    [AllowAnonymous]
    [HttpGet("HomeAny")]
    public IActionResult HomeAnyIndex(int Page = 1)
    {
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeanyDBService.GetIdList(Data.Paging);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            // 宣告一個新陣列內物件
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            Data.RentalBlock.Add(newBlock);
        }
        return Ok(Data);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult AnyReadImg(Guid Id)
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
}