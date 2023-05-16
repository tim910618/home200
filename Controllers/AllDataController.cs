
using api1.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api1.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AllDataController : ControllerBase
    {
        private readonly AllDataDBService _alldataSerivce;
        public AllDataController(AllDataDBService alldataSerivce)
        {
            _alldataSerivce = alldataSerivce;
        }

        /*房東*/
        //型態
        [AllowAnonymous]
        [HttpGet("AllDataHomegenre")]
        public IActionResult AllDataHomegenre()
        {
            List<string> Data=_alldataSerivce.AllHomegenre();
            return Ok(Data);
        }

        //類型
        [AllowAnonymous]
        [HttpGet("AllDataHometype")]
        public IActionResult AllDataHometype()
        {
            List<string> Data=_alldataSerivce.AllHometype();
            return Ok(Data);
        }

        //地區
        [AllowAnonymous]
        [HttpGet("AllDataHomeaddress")]
        public IActionResult AllDataHomeaddress()
        {
            List<string> Data=_alldataSerivce.AllHomeaddress();
            return Ok(Data);
        }


        /*管理者*/

    }

}