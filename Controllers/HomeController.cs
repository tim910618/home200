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
    public HomeController(HomeDBService homeDBService, IWebHostEnvironment env, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _homeDBService = homeDBService;
        _env = env;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }


    //上架 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeUp")]
    public IActionResult HomeUpIndex()
    {
        var publisher = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        HomeListViewModel Data = new HomeListViewModel();
        Data.IdList = _homeDBService.GetUpIdList(publisher);
        Data.RentalBlock = new List<HomeViewModel>();
        foreach (var Id in Data.IdList)
        {
            HomeViewModel newBlock = new HomeViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            newBlock.Image=_homeDBService.GetImgById(Id);
            if (newBlock.AllData.isDelete == false || newBlock.AllData.tenant == true)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if (Data.RentalBlock.Count == 0)
        {
            return Ok("無資料");
        }
        return Ok(Data);
    }
    //下架
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeDown")]
    public IActionResult HomeDownIndex()
    {
        var publisher = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        HomeListViewModel Data = new HomeListViewModel();
        Data.IdList = _homeDBService.GetDownIdList(publisher);
        Data.RentalBlock = new List<HomeViewModel>();
        foreach (var Id in Data.IdList)
        {
            HomeViewModel newBlock = new HomeViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            newBlock.Image=_homeDBService.GetImgById(Id);
            if (newBlock.AllData.isDelete == false || newBlock.AllData.check != 0 && newBlock.AllData.tenant == false)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if (Data.RentalBlock.Count == 0)
        {
            return Ok("無資料");
        }
        return Ok(Data);
    }
    //審核中
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpGet("HomeCheck")]
    public IActionResult HomeCheckIndex()
    {
        var publisher = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        HomeListViewModel Data = new HomeListViewModel();
        Data.IdList = _homeDBService.GetCheckIdList(publisher);
        Data.RentalBlock = new List<HomeViewModel>();
        foreach (var Id in Data.IdList)
        {
            HomeViewModel newBlock = new HomeViewModel();
            newBlock.AllData = _homeDBService.GetDataById(Id);
            newBlock.Image=_homeDBService.GetImgById(Id);
            if (newBlock.AllData.isDelete == false || (newBlock.AllData.check == 0) && newBlock.AllData.tenant == false)
            {
                Data.RentalBlock.Add(newBlock);
            }
        }
        if (Data.RentalBlock.Count == 0)
        {
            return Ok("無資料");
        }
        return Ok(Data);
    }
    //上架變下架
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("HomeUpToDown/{id:guid}")]
    public IActionResult HomeUpToDown(Guid Id, [FromForm] Rental UpToDownData)
    {
        UpToDownData.rental_id = Id;
        _homeDBService.UpToDown(UpToDownData);
        return Ok("已下架");
    }
    //送出審查
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("HomeDownToCheck/{id:guid}")]
    public IActionResult HomeDownToCheck(Guid Id, [FromForm] Rental DownToCheckData)
    {
        DownToCheckData.rental_id = Id;
        _homeDBService.DownToCheck(DownToCheckData);
        return Ok("已送出審查");
    }



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPost("InsertRental")]
    public IActionResult InsertRental([FromForm] Rental Data)
    {
        Data.publisher = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        try
        {
            // 檢查檔案是否存在
            if (Data.img1_1 == null || Data.img1_1.Length == 0)
                return BadRequest("請選擇一張照片");
            if (Data.titledeed_1== null || Data.img1_1.Length == 0)
                return BadRequest("請選擇一張照片");

            string uploadPath = _configuration.GetSection("UploadPath").Value;
            string uploadFolderPath = Path.Combine(uploadPath, "Uploads");

            //不存在就新增資料夾
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 檢查檔案類型是否為圖片
            if (!Data.img1_1.ContentType.StartsWith("image/"))
                return BadRequest("檔案格式不正確，請上傳圖片檔案");


            // 一張地契
            if (Data.titledeed_1!=null && Data.titledeed_1.Length>0)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(Data.titledeed_1.FileName);
                // 儲存檔案
                var path = Path.Combine(uploadPath, filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Data.titledeed_1.CopyTo(stream);
                }
                Data.titledeed = filename;
            }

            // 很多張圖片
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
            return Ok("新增成功");
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
                if (Data.isDelete == true)
                {
                    return Ok("已被刪除");
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

    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateImgAsync(Guid Id, [FromForm] Rental updateData)
    {
        var data = _homeDBService.GetDataById(Id);

        if (data.isDelete == true)
        {
            return Ok("查無此資訊");
        }

        if(updateData.img1_1 != null)
        {
            _homeDBService.OldFileCheck(data.img1);
            updateData.img1 = _homeDBService.CreateOneImage(updateData.img1_1);
        }
        else
            updateData.img1=data.img1;
        if(updateData.img1_2 != null)
        {
            _homeDBService.OldFileCheck(data.img2);
            updateData.img2 = _homeDBService.CreateOneImage(updateData.img1_2);
        }
        else
            updateData.img2=data.img2;
        if(updateData.img1_3 != null)
        {
            _homeDBService.OldFileCheck(data.img3);
            updateData.img3 = _homeDBService.CreateOneImage(updateData.img1_3);
        }
        else
            updateData.img3=data.img3;
        if(updateData.img1_4 != null)
        {
            _homeDBService.OldFileCheck(data.img4);
            updateData.img4 = _homeDBService.CreateOneImage(updateData.img1_4);
        }
        else
            updateData.img4=data.img4;
        if(updateData.img1_5 != null)
        {
            _homeDBService.OldFileCheck(data.img5);
            updateData.img5 = _homeDBService.CreateOneImage(updateData.img1_5);
        }
        else
            updateData.img5=data.img5;
        //地契
        if(updateData.titledeed_1 != null)
        {
            _homeDBService.OldFileCheck(data.titledeed);
            updateData.titledeed = _homeDBService.CreateOneImage(updateData.titledeed_1);
        }
        else
            updateData.titledeed=data.titledeed;

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

    /*var imgPathList = new List<string>();
    for (int i = 1; i <= 5; i++)
    {
        var imgPath = newBlock.AllData.GetType().GetProperty($"img{i}").GetValue(newBlock.AllData) as string;
        if (!string.IsNullOrEmpty(imgPath))
        {
            imgPathList.Add($"{Request.Scheme}://{Request.Host.Value}/{imgPath.Replace("\\", "/")}");
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
    }*/
    /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "publisher")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateImgAsync(Guid Id, [FromForm] Rental updateData)
    {
        var data = _homeDBService.GetDataById(Id);

        if (data.isDelete == true)
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

                if (!string.IsNullOrEmpty(oldFilePath) && oldFilePath.Contains("http://localhost:5190/Image/"))
                {
                    oldFilePath = oldFilePath.Replace("http://localhost:5190/Image/", "");
                }

                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                var path = Path.Combine(uploadPath, filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await files[i].CopyToAsync(stream);
                    //files[i].CopyToAsync(stream);
                }

                var newImgUrl = Path.Combine(uploadFolderPath, filename);
                updateData.GetType().GetProperty($"img{i + 1}").SetValue(updateData, newImgUrl, null);
            }
        }

        /*updateData.img1 = filenames.Count > 0 ? filenames[0].Replace("http://localhost:5190/Image/", "") : data.img1;
        updateData.img2 = filenames.Count > 1 ? filenames[1].Replace("http://localhost:5190/Image/", "") : data.img2;
        updateData.img3 = filenames.Count > 2 ? filenames[2].Replace("http://localhost:5190/Image/", "") : data.img3;
        updateData.img4 = filenames.Count > 3 ? filenames[3].Replace("http://localhost:5190/Image/", "") : data.img4;
        updateData.img5 = filenames.Count > 4 ? filenames[4].Replace("http://localhost:5190/Image/", "") : data.img5;

        updateData.img1 = filenames.Count > 0 ? filenames[0] : data.img1;
        updateData.img2 = filenames.Count > 1 ? filenames[1] : data.img2;
        updateData.img3 = filenames.Count > 2 ? filenames[2] : data.img3;
        updateData.img4 = filenames.Count > 3 ? filenames[3] : data.img4;
        updateData.img5 = filenames.Count > 4 ? filenames[4] : data.img5;

        updateData.rental_id = Id;
        _homeDBService.UpdateImgData(updateData);

        return Ok(updateData);
    }*/


}
