// using Microsoft.AspNetCore.Mvc;
// using api1.Service;
// using api1.ViewModel;
// using Microsoft.Data.SqlClient;
// using api1.Models;

// namespace api1.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class MembersController : ControllerBase
// {
//     private readonly MembersDBService _membersSerivce;

//     public MembersController(MembersDBService membersSerivce)
//     {
//         _membersSerivce = membersSerivce;
//     }
//     private MailService mailService=new MailService();

//     // [HttpGet("{acc}", Name = "GetMembers")]
//     [HttpGet]
//     [Route("GetMembers")]
//     public IActionResult GetMembers(string acc)
//     {
//         var members = _membersSerivce.GetDataByAccount(acc);
//         if(members==null){
//             return Ok("查無此人");
//         };
//         return Ok(members);
//     }

//     [HttpGet(Name = "GetMembersList")]
//     public IActionResult GetMembersList()
//     {
//         List<Members> DataList=_membersSerivce.GetDataByAccountList();
//         return Ok(DataList);
//     }

//     [HttpPost("register")]
//     public IActionResult Register([FromBody] MembersRegisterViewModel registerMember)
//     {
//         if (ModelState.IsValid)
//         {
//             registerMember.newMember.password = registerMember.password;
//             string authCode = mailService.GetValidateCode();
//             registerMember.newMember.authcode = authCode;
//             _membersSerivce.Register(registerMember.newMember);
//             string tempMail = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views/Shared/RegisterEmail.html"));
//             UriBuilder validateUrl = new UriBuilder(Request.Scheme, Request.Host.Host)
//             {
//                 Path = Url.Action("EmailValidate", "Members", new { account = registerMember.newMember.account, authCode = registerMember.newMember.authcode })
//             };
//             string mailBody = mailService.GetRegisterMailBody(tempMail, registerMember.newMember.name, validateUrl.ToString().Replace("%3F", "?"));
//             mailService.SendRegisterMail(mailBody, registerMember.newMember.email);
//             return Ok(new { message = "註冊成功，請去收信以來驗證EMAIL" });
//         }
//         registerMember.password = null;
//         registerMember.passwordCheck = null;
//         return BadRequest(ModelState);
//     }

// }