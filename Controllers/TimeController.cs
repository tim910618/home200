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
        public TimeController(TimeDBService timeDBService)
        {
            _timeService=timeDBService;

        }

    #region 房東設定可預約時間
    [HttpPost("AddBookTime")]
    public IActionResult AddBookTime([FromBody] BookTime bookTime)
    {
        bookTime.publisher="admin";
        // bookTime.publisher=User.Identity.Name;
        // if(User.Identity.Name==null){
        //     return BadRequest("請去登入");
        // }
        _timeService.AddBookTime(bookTime);
        return Ok();
    }
    #endregion

    #region 取得所選時間清單
    
    #endregion

    #region 取得已預約時間清單
    #endregion

    #region 新增預約
    
    #endregion

    #region 取消預約
    #endregion


}
