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
public class HomeController : ControllerBase
{
    private readonly HomeDBService _homeDBService;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    private readonly IHttpContextAccessor _httpContextAccessor;
    public HomeController(HomeDBService homeDBService, IWebHostEnvironment env, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
    {
        _homeDBService = homeDBService;
        _env = env;
        _configuration = configuration;
        _httpContextAccessor=httpContextAccessor;
    }

    
    //上架 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeUp")]
    public IActionResult HomeUpIndex(int Page = 1)
    {
        var publisher=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeDBService.GetUpIdList(Data.Paging,publisher);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if(newBlock.AllData.isDelete==false || newBlock.AllData.tenant==true)
            {
                var imgPathList = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    var imgPath = newBlock.AllData.GetType().GetProperty($"img{i}").GetValue(newBlock.AllData) as string;
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        imgPathList.Add($"http://localhost:5555/{imgPath.Replace("\\", "/")}");
                    }
                }
                string ImagePath = string.Join(",", imgPathList);
                
                string[] imagePaths = ImagePath.Split(',');
                if (imagePaths.Length >= 1) 
                {
                    newBlock.AllData.img1 = imagePaths[0];
                }
                if (imagePaths.Length >= 2) 
                {
                    newBlock.AllData.img2 = imagePaths[1];
                }
                Data.RentalBlock.Add(newBlock);
            }
        }
        return Ok(Data);
    }
    //下架
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeDown")]
    public IActionResult HomeDownIndex(int Page = 1)
    {
        var publisher=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeDBService.GetDownIdList(Data.Paging,publisher);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if(newBlock.AllData.isDelete==false || newBlock.AllData.check==1 && newBlock.AllData.tenant==false)
            {
                var imgPathList = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    var imgPath = newBlock.AllData.GetType().GetProperty($"img{i}").GetValue(newBlock.AllData) as string;
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        imgPathList.Add($"http://localhost:5555/{imgPath.Replace("\\", "/")}");
                    }
                }
                string ImagePath = string.Join(",", imgPathList);
                
                string[] imagePaths = ImagePath.Split(',');
                if (imagePaths.Length >= 1) 
                {
                    newBlock.AllData.img1 = imagePaths[0];
                }
                if (imagePaths.Length >= 2) 
                {
                    newBlock.AllData.img2 = imagePaths[1];
                }
                Data.RentalBlock.Add(newBlock);
            }
        }
        return Ok(Data);
    }
    //審核中
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeCheck")]
    public IActionResult HomeCheckIndex(int Page = 1)
    {
        var publisher=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        RentalListViewModel Data = new RentalListViewModel();
        Data.Paging = new ForPaging(Page);
        Data.IdList = _homeDBService.GetCheckIdList(Data.Paging,publisher);
        Data.RentalBlock = new List<RentaldetailViewModel>();
        foreach (var Id in Data.IdList)
        {
            RentaldetailViewModel newBlock = new RentaldetailViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            if(newBlock.AllData.isDelete==false || (newBlock.AllData.check == 0 || newBlock.AllData.check == 2) && newBlock.AllData.tenant==false)
            {
                var imgPathList = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    var imgPath = newBlock.AllData.GetType().GetProperty($"img{i}").GetValue(newBlock.AllData) as string;
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        imgPathList.Add($"http://localhost:5555/{imgPath.Replace("\\", "/")}");
                    }
                }
                string ImagePath = string.Join(",", imgPathList);
                
                string[] imagePaths = ImagePath.Split(',');
                if (imagePaths.Length >= 1) 
                {
                    newBlock.AllData.img1 = imagePaths[0];
                }
                if (imagePaths.Length >= 2) 
                {
                    newBlock.AllData.img2 = imagePaths[1];
                }
                Data.RentalBlock.Add(newBlock);
            }
        }
        return Ok(Data);
    }
    //上架變下架
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("HomeUpToDown/{id:guid}")]
    public IActionResult HomeUpToDown(Guid Id, [FromForm] Rental UpToDownData)
    {
        UpToDownData.rental_id=Id;
        _homeDBService.UpToDown(UpToDownData);
        return Ok("已下架");
    }
    //送出審查
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("HomeDownToCheck/{id:guid}")]
    public IActionResult HomeDownToCheck(Guid Id, [FromForm] Rental DownToCheckData)
    {
        DownToCheckData.rental_id=Id;
        _homeDBService.DownToCheck(DownToCheckData);
        return Ok("已送出審查");
    }

    

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPost("InsertRental")]
    public IActionResult InsertRental([FromForm] Rental Data)
    {
        Data.publisher=_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        try
        {
            // 檢查檔案是否存在
            if (Data.img1_1 == null || Data.img1_1.Length == 0)
                return BadRequest("請選擇一張照片");

            string uploadPath = _configuration.GetSection("UploadPath").Value;
            string uploadFolderPath = Path.Combine(uploadPath, "Uploads");

            //不存在就新增資料夾
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 檢查檔案類型是否為圖片
            if (!Data.img1_1.ContentType.StartsWith("image/"))
                return BadRequest("檔案格式不正確，請上傳圖片檔案");

            // 產生檔名，以避免重複
            List<string> filenames = new List<string>();
            IFormFile[] files = { Data.img1_1, Data.img1_2, Data.img1_3, Data.img1_4, Data.img1_5 };
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != null && files[i].Length > 0)
                {
                    // 產生檔名，以避免重複
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(files[i].FileName);
                    filenames.Add(filename);

                    // 儲存檔案
                    var path = Path.Combine(uploadPath, filename);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        files[i].CopyTo(stream);
                    }
                }
            }
            Data.img1 = filenames.Count > 0 ? filenames[0] : null;
            Data.img2 = filenames.Count > 1 ? filenames[1] : null;
            Data.img3 = filenames.Count > 2 ? filenames[2] : null;
            Data.img4 = filenames.Count > 3 ? filenames[3] : null;
            Data.img5 = filenames.Count > 4 ? filenames[4] : null;

            _homeDBService.InsertHouse_Rental(Data);
            return Ok(filenames);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("{id}")]
    public IActionResult ReadImg(Guid Id)
    {
        try
        {
            Rental Data = _homeDBService.GetDataById(Id);

            if (Data != null)
            {
                if(Data.isDelete==true)
                {
                    return Ok("已被刪除");
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



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]

    [HttpPut("{id:guid}")]
    public IActionResult UpdateImg(Guid Id, [FromForm] Rental updateData)
    {
        var data = _homeDBService.GetDataById(Id);

        if (data.isDelete==true)
        {
            return Ok("查無此資訊");
        }

        string uploadPath = _configuration.GetSection("UploadPath").Value;
        string uploadFolderPath = Path.Combine(uploadPath, "Uploads");
        // 產生檔名，以避免重複
        List<string> filenames = new List<string>();
        IFormFile[] files = { updateData.img1_1, updateData.img1_2, updateData.img1_3, updateData.img1_4, updateData.img1_5 };
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] != null && files[i].Length > 0)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(files[i].FileName);
                filenames.Add(filename);

                // 刪除原本的檔案
                string oldFilePath = null;
                switch (i + 1)
                {
                    case 1:
                        oldFilePath = data.img1;
                        break;
                    case 2:
                        oldFilePath = data.img2;
                        break;
                    case 3:
                        oldFilePath = data.img3;
                        break;
                    case 4:
                        oldFilePath = data.img4;
                        break;
                    case 5:
                        oldFilePath = data.img5;
                        break;
                }
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                var path = Path.Combine(uploadPath, filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    files[i].CopyToAsync(stream);
                }
            }
        }
        updateData.img1 = filenames.Count > 0 ? filenames[0] : data.img1;
        updateData.img2 = filenames.Count > 1 ? filenames[1] : data.img2;
        updateData.img3 = filenames.Count > 2 ? filenames[2] : data.img3;
        updateData.img4 = filenames.Count > 3 ? filenames[3] : data.img4;
        updateData.img5 = filenames.Count > 4 ? filenames[4] : data.img5;

        updateData.rental_id = Id;
        _homeDBService.UpdateImgData(updateData);

        return Ok(updateData);
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteImg(Guid id)
    {
        _homeDBService.DeleteImgData(id);
        return Ok("刪除成功");
    }

    /////////////////////////////////////////////////////////////////////
    /*[HttpPost("Img")]
    public async Task<IActionResult> Img([FromForm] IFormFile file)
    {
        try
        {
            // 檢查檔案是否存在
            if (file == null || file.Length == 0)
                return BadRequest("請選擇一張照片");
            string uploadPath = _configuration.GetSection("UploadPath").Value;
            string uploadFolderPath = Path.Combine(uploadPath, "Uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 檢查檔案類型是否為圖片
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("檔案格式不正確，請上傳圖片檔案");

            // 產生檔名，以避免重複
            string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // 儲存檔案
            var path = Path.Combine(uploadPath, filename);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //_homeDBService.InsertHouse_Rental(new img { img1 = await GetFileBytes(file) });
            return Ok(filename);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    private async Task<byte[]> GetFileBytes(IFormFile file)
    {
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }
    }*/
}
