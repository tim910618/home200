/*
using Microsoft.AspNetCore.Mvc;
using api1.Service;
using api1.ViewModel;
using api1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace api1.Controllers;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class RentalController : ControllerBase
{
    private readonly RentalService _rentalDBService;
    private readonly ForPaging _forPaging;
    private readonly IWebHostEnvironment _env;
    public RentalController(RentalService rentalDBService, ForPaging forPaging,IWebHostEnvironment env)
    {
        _rentalDBService = rentalDBService;
        _forPaging = forPaging;
        _env = env;
    }
        [HttpPost("try")]
        public void Post(InsertRentalViewModel Data,ICollection<IFormFile> files)
        {
            var rootPath = _env.ContentRootPath + @"\Upload\";
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = file.FileName;
                    using (var stream = System.IO.File.Create(rootPath + filePath))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
        }
    #region 單筆房屋資訊OK
    [HttpGet("{id}")]
    public IActionResult GetRental(Guid id)
    {
        var rental = _rentalDBService.GetRentalById(id);
        if (rental == null) //防呆防呆
        {
            return NotFound("查無房屋資訊");
        }
        return Ok(rental);
    }
    #endregion

    #region 新增房屋資訊
    [HttpPost("InsertRental")]
    public IActionResult InsertRental([FromBody] InsertRentalViewModel Data,List<IFormFile> files)
    {
        Data.newRental.Member.account = User.Identity.Name;

        for (int i = 0; i < files.Count; i++)
        {
            if (files[i] != null)
            {
                string filename = Path.GetFileName(files[i].FileName);
                string uploadsFolder = Path.Combine(_env.WebRootPath, "Upload");
                string filePath = Path.Combine(uploadsFolder, filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    files[i].CopyTo(stream);
                }
                switch (i)
                {
                    case 0:
                        Data.newRental.img1 = filename;
                        break;
                    case 1:
                        Data.newRental.img2 = filename;
                        break;
                    case 2:
                        Data.newRental.img3 = filename;
                        break;
                    case 3:
                        Data.newRental.img4 = filename;
                        break;
                    case 4:
                        Data.newRental.img5 = filename;
                        break;
                    case 5:
                        Data.newRental.img6 = filename;
                        break;
                    case 6:
                        Data.newRental.img7 = filename;
                        break;
                    case 7:
                        Data.newRental.img8 = filename;
                        break;
                }
            }
        }
        if (Data.newRental.img1 == null)
        {
            return BadRequest("照片1為必填");
        }
        _rentalDBService.InsertHouse_Rental(Data.newRental);
        return Ok("新增成功");
    }
    #endregion

    #region 修改房屋資訊
    [HttpPut("Upload")]
    [Authorize(Roles = "Publisher")]
    public IActionResult Update_Rental([FromQuery] Guid Rental_Id, [FromQuery] Rental UpdateData)
    {
        Rental Data = _rentalDBService.GetRentalById(Rental_Id);
        if (UpdateData == null)
        {
            return NotFound("未找到此房屋");
        }
        Data.genre = UpdateData.genre;
        Data.pattern = UpdateData.pattern;
        Data.type = UpdateData.type;
        Data.title = UpdateData.title;
        Data.address = UpdateData.address;
        Data.rent = UpdateData.rent;
        Data.waterfee = UpdateData.waterfee;
        Data.electricitybill = UpdateData.electricitybill;
        Data.adminfee = UpdateData.adminfee;
        Data.floor = UpdateData.floor;
        Data.area = UpdateData.area;
        Data.equipmentname = UpdateData.equipmentname;
        Data.content = UpdateData.content;
        Data.img1 = UpdateData.img1;
        Data.img2 = UpdateData.img2;
        Data.img3 = UpdateData.img3;
        Data.img4 = UpdateData.img4;
        Data.img5 = UpdateData.img5;
        Data.img6 = UpdateData.img6;
        Data.img7 = UpdateData.img7;
        Data.img8 = UpdateData.img8;
        _rentalDBService.UpdateRental(Data);
        return Ok("房屋資訊更新成功");
    }
    #endregion

    #region 刪除房屋OK
    [HttpPut("Delete")]
    //[Authorize(Roles = "Admin")]
    public IActionResult Delete_rental([FromQuery] string Id = "")
    {
        if (string.IsNullOrEmpty(Id))
        {
            return BadRequest("請提供房屋編號");
        }
        if (!Guid.TryParse(Id, out Guid rentalId))
        {
            return BadRequest("請提供正確房屋編號");
        }
        Rental Data = new Rental();
        Data = _rentalDBService.GetRentalById(rentalId);
        string Str = _rentalDBService.DeleteRental(Data);

        if (Str == "此房屋不存在" || Str == "此房屋已被刪除")
        {
            return BadRequest(Str);
        }

        return Ok(Str);
    }
    #endregion

    #region 審核通過OK

    [HttpPut("Check")]
    //[Authorize(Roles = "Admin")]
    public IActionResult Check_Rental([FromQuery] string Id = "")
    {
        if (string.IsNullOrEmpty(Id))
        {
            return BadRequest("請提供房屋編號");
        }
        if (!Guid.TryParse(Id, out Guid rentalId))
        {
            return BadRequest("請提供正確房屋編號");
        }
        Rental Data = new Rental();
        Data = _rentalDBService.GetRentalById(rentalId);
        string Str = _rentalDBService.CheckUpload(Data);

        if (Str == "此房屋不存在" || Str == "此房屋已審核")
        {
            return BadRequest(Str);
        }

        return Ok(Str);
    }
    #endregion

    

    #region 下架、出租OK
    [HttpPut("tackoff")]
    //[Authorize(Roles = "Publisher")]
    public IActionResult tackoff_Rental([FromQuery] string Id = "")
    {
        if (string.IsNullOrEmpty(Id))
        {
            return BadRequest("請提供房屋編號");
        }
        if (!Guid.TryParse(Id, out Guid rentalId))
        {
            return BadRequest("請提供正確房屋編號");
        }
        Rental Data = new Rental();
        Data = _rentalDBService.GetRentalById(rentalId);
        string Str = _rentalDBService.Tackoff(Data);

        if (Str == "此房屋不存在" || Str == "此房屋已下架")
        {
            return BadRequest(Str);
        }

        return Ok(Str);
    }
    #endregion

    //收藏需要另外資料表
    #region 收藏清單OK
    [HttpGet("FavoriteRentalList")]
    public IActionResult FavoriteRental([FromQuery] int Page = 1)
    {
        ForPaging paging = new ForPaging();
        paging.NowPage = Page;
        string account = User.Identity.Name;
        account = "test";
        _rentalDBService.SetFavoriteMaxPaging(account, paging);
        var RentalList = _rentalDBService.GetFavoriteDataList(account, paging);
        if (RentalList.Count == 0)
        {
            return Ok("收藏房屋空空如也");
        }
        return Ok(RentalList);
    }
    #endregion

    #region 加入收藏OK
    [HttpPost("AddFavoriteRental")]
    public IActionResult FavoriteRental([FromQuery] Guid id)
    {
        Rental Data = _rentalDBService.GetRentalById(id);
        string Account = User.Identity.Name;
        Account = "test";
        if (Data == null) //防呆
        {
            return Ok("查無房屋資訊");
        }
        //要判斷有沒有collect重複
        _rentalDBService.AddFavorite(Account, Data.rental_id);
        return Ok("收藏成功");
    }
    #endregion

    #region 取消收藏OK
    [HttpDelete("RemoveFavorite")]
    public IActionResult RemoveFavorite([FromQuery] Guid id)
    {
        Rental Data = _rentalDBService.GetRentalById(id);
        string account = User.Identity.Name;
        account = "test";
        if (Data == null) //防呆
        {
            return NotFound("查無房屋資訊");
        }
        _rentalDBService.RemoveFavorite(account, Data.rental_id);
        return Ok("取消收藏成功");
    }
    #endregion


    //////////////////////////////////////////////////////////////////////////////////////////////////
    #region 房屋列表-首頁
    [HttpGet("AllRentalList")]
    public IActionResult GetAllRentalList([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] string filter = "")
    {
        RentalListViewModel RentalList = new RentalListViewModel();
        RentalList.Paging = new ForPaging(page);

        if (filter == "Checked" && User.IsInRole("Admin"))
        {
            RentalList.DataList = _rentalDBService.GetCheckDataList(search, RentalList.Paging);
        }
        else if (filter == "Unchecked" && User.IsInRole("Admin"))
        {
            RentalList.DataList = _rentalDBService.GetUncheckDataList(search, RentalList.Paging);
        }
        else if (filter == "Published" && User.IsInRole("Publisher"))
        {
            RentalList.DataList = _rentalDBService.GetTakeoffDataList(User.Identity.Name, search, RentalList.Paging);
        }
        else if (filter == "Unpublished" && User.IsInRole("Publisher"))
        {
            RentalList.DataList = _rentalDBService.GetPutOnDataList(User.Identity.Name, search, RentalList.Paging);
        }
        else if (filter == "Unchecked" && User.IsInRole("Publisher"))
        {
            RentalList.DataList = _rentalDBService.GetUncheckDataList(User.Identity.Name, search, RentalList.Paging);
        }
        else
        {
            RentalList.DataList = _rentalDBService.GetDataList(search, RentalList.Paging);
        }

        if (RentalList.DataList == null)
        {
            return BadRequest("查無房屋");
        }

        return Ok(RentalList.DataList);
    }
    #endregion

}
*/