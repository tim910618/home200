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
        public ListController(ListDBService listDBService)
        {
            _ListService=listDBService;

        }

    #region 取得已預約時間清單
    [HttpGet]
    public IActionResult GetBookList()
    {
        string account="renter123";
        // bookTime.publisher=User.Identity.Name;
        // if(User.Identity.Name==null){
        //     return BadRequest("請去登入");
        // }
        List<BookList> DataList= _ListService.GetBookTime(account);
        if(DataList==null)
        {
            return Ok("無預約資訊");
        }
        return Ok(DataList);
    }
    #endregion

    #region 新增預約
    public IActionResult AddBooking()
    {
        string account="";
        return Ok();
    }
    #endregion

    #region 取消預約
    #endregion


}
