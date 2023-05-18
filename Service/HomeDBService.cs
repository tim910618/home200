using Microsoft.Data.SqlClient;
using api1.Models;
using System.Data;

namespace api1.Service
{
    public class HomeDBService
    {
        private readonly SqlConnection conn;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeDBService(SqlConnection connection, IHttpContextAccessor httpContextAccessor)
        {
            conn = connection;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<Guid> GetUpIdList(string publisher)
        {
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE publisher = @publisher AND tenant = 1 AND [check] = 1 AND isDelete = 0) m ORDER BY m.sort;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Guid.Parse(dr["rental_id"].ToString()));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return IdList;
        }
        public List<Guid> GetDownIdList(string publisher)
        {
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE publisher = @publisher AND tenant = 0 AND [check] IN (1, 2) AND isDelete = 0) m ORDER BY m.sort;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Guid.Parse(dr["rental_id"].ToString()));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return IdList;
        }
        public List<Guid> GetCheckIdList(string publisher)
        {
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE publisher = @publisher AND tenant = 0 AND [check] = 0 AND isDelete = 0) m ORDER BY m.sort;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Guid.Parse(dr["rental_id"].ToString()));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return IdList;
        }
        public void SetMaxPaging(ForPaging Paging, string publisher)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM RENTAL WHERE publisher = @publisher; ";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.Item));
            Paging.SetRightPage();
        }


        public void UpToDown(Rental UpToDownData)
        {
            string sql = $@"UPDATE RENTAL SET [check]=@check ,tenant=@tenant WHERE rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", UpToDownData.rental_id);
                cmd.Parameters.AddWithValue("@check", 1);
                cmd.Parameters.AddWithValue("@tenant", false);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        public void DownToCheck(Rental DownToCheckData)
        {
            string sql = $@"UPDATE RENTAL SET [check]=@check ,tenant=@tenant WHERE rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", DownToCheckData.rental_id);
                cmd.Parameters.AddWithValue("@check", 0);
                cmd.Parameters.AddWithValue("@tenant", false);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }





        public Rental GetDataById(Guid Id)
        {
            Rental Data = new Rental();
            string sql = $@"SELECT m.*,d.* FROM RENTAL m INNER JOIN MEMBERS d ON m.publisher = d.Account WHERE m.rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.rental_id = (Guid)dr["rental_id"];
                Data.publisher = dr["publisher"].ToString();
                Data.genre = dr["genre"].ToString();
                Data.pattern = dr["pattern"].ToString();
                Data.type = dr["type"].ToString();
                Data.title = dr["title"].ToString();
                Data.address = dr["address"].ToString();
                Data.rent = Convert.ToInt32(dr["rent"]);
                Data.waterfee = Convert.ToInt32(dr["waterfee"]);
                Data.electricitybill = Convert.ToInt32(dr["electricitybill"]);
                Data.adminfee = Convert.ToInt32(dr["adminfee"]);
                Data.floor = dr["floor"].ToString();
                Data.area = Convert.ToInt32(dr["area"]);
                Data.content = dr["content"].ToString();
                Data.equipmentname = dr["equipmentname"].ToString();
                Data.img1 = dr["img1"].ToString();
                Data.img2 = dr["img2"].ToString();
                Data.img3 = dr["img3"].ToString();
                Data.img4 = dr["img4"].ToString();
                Data.img5 = dr["img5"].ToString();
                Data.titledeed=dr["titledeed"].ToString();
                Data.check = Convert.ToInt32(dr["check"]);
                Data.tenant = Convert.ToBoolean(dr["tenant"]);
                Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
                Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
                Data.reason = dr["reason"].ToString();
                Data.Member.account = dr["account"].ToString();
                Data.Member.name = dr["name"].ToString();
                Data.Member.score = Convert.ToDouble(dr["score"]);
                Data.Member.phone = dr["phone"].ToString();
                Data.Member.isBlock = Convert.ToBoolean(dr["isBlock"]);
                Data.Member.email = dr["email"].ToString();
                var imgPathtitledeed = dr["titledeed"].ToString();
                if (!string.IsNullOrEmpty(imgPathtitledeed))
                {
                    if (imgPathtitledeed.StartsWith("http://"))
                    {
                        Data.titledeed = imgPathtitledeed;
                    }
                    else
                    {
                        Data.titledeed = $"http://localhost:5190/Image/{imgPathtitledeed.Replace("\\", "/")}";
                    }
                }
                var imgPathList = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    var imgPath = dr[$"img{i}"].ToString();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        if (!imgPath.Contains("http://"))
                        {
                            imgPath = $"http://localhost:5190/Image/{imgPath.Replace("\\", "/")}";
                        }
                        imgPathList.Add(imgPath);
                    }
                }
                var imagePaths = imgPathList.ToArray();
                if (imagePaths.Length >= 1)
                {
                    Data.img1 = imagePaths[0];
                }
                if (imagePaths.Length >= 2)
                {
                    Data.img2 = imagePaths[1];
                }
                if (imagePaths.Length >= 3)
                {
                    Data.img3 = imagePaths[2];
                }
                if (imagePaths.Length >= 4)
                {
                    Data.img4 = imagePaths[3];
                }
                if (imagePaths.Length >= 5)
                {
                    Data.img5 = imagePaths[4];
                }

            }
            catch (Exception)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if (Data == null || Data.Member.isBlock == true)
            {
                return null;
            }
            else
            {
                return Data;
            }
        }
        public string[] GetImgById(Guid Id)
        {
            Rental Data = new Rental();
            string sql = $@"SELECT m.*,d.* FROM RENTAL m INNER JOIN MEMBERS d ON m.publisher = d.Account WHERE m.rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.img1 = dr["img1"].ToString();
                Data.img2 = dr["img2"].ToString();
                Data.img3 = dr["img3"].ToString();
                Data.img4 = dr["img4"].ToString();
                Data.img5 = dr["img5"].ToString();
                Data.Member.isBlock = Convert.ToBoolean(dr["isBlock"]);
                var imgPathList = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    var imgPath = dr[$"img{i}"].ToString();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        if (!imgPath.Contains("http://"))
                        {
                            imgPath = $"http://localhost:5190/Image/{imgPath.Replace("\\", "/")}";
                        }
                        imgPathList.Add(imgPath);
                    }
                }
                if (Data == null || Data.Member.isBlock == true)
                {
                    return null;
                }
                else
                {
                    return imgPathList.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        public void UpdateImgData(Rental UpdateData)
        {

            string sql = $@"UPDATE RENTAL SET genre=@genre, pattern=@pattern, type=@type, title=@title, address=@address, rent=@rent, waterfee=@waterfee, electricitybill=@electricitybill, adminfee=@adminfee, floor=@floor, area=@area,
                equipmentname=@equipmentname, content=@content, img1=@img1, img2=@img2,img3=@img3,img4=@img4,img5=@img5,titledeed=@titledeed,uploadtime=@uploadtime ,[check]=@check,tenant=@tenant
                WHERE rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", UpdateData.rental_id);
                cmd.Parameters.AddWithValue("@genre", UpdateData.genre);
                cmd.Parameters.AddWithValue("@pattern", UpdateData.pattern);
                cmd.Parameters.AddWithValue("@type", UpdateData.type);
                cmd.Parameters.AddWithValue("@title", UpdateData.title);
                cmd.Parameters.AddWithValue("@address", UpdateData.address);
                cmd.Parameters.AddWithValue("@rent", UpdateData.rent);
                cmd.Parameters.AddWithValue("@waterfee", UpdateData.waterfee);
                cmd.Parameters.AddWithValue("@electricitybill", UpdateData.electricitybill);
                cmd.Parameters.AddWithValue("@adminfee", UpdateData.adminfee);
                cmd.Parameters.AddWithValue("@floor", UpdateData.floor);
                cmd.Parameters.AddWithValue("@area", UpdateData.area);
                cmd.Parameters.AddWithValue("@equipmentname", UpdateData.equipmentname);
                cmd.Parameters.AddWithValue("@content", UpdateData.content);
                cmd.Parameters.AddWithValue("@img1", string.IsNullOrEmpty(UpdateData.img1) ? DBNull.Value : (object)UpdateData.img1);
                cmd.Parameters.AddWithValue("@img2", string.IsNullOrEmpty(UpdateData.img2) ? DBNull.Value : (object)UpdateData.img2);
                cmd.Parameters.AddWithValue("@img3", string.IsNullOrEmpty(UpdateData.img3) ? DBNull.Value : (object)UpdateData.img3);
                cmd.Parameters.AddWithValue("@img4", string.IsNullOrEmpty(UpdateData.img4) ? DBNull.Value : (object)UpdateData.img4);
                cmd.Parameters.AddWithValue("@img5", string.IsNullOrEmpty(UpdateData.img5) ? DBNull.Value : (object)UpdateData.img5);
                cmd.Parameters.AddWithValue("@titledeed", string.IsNullOrEmpty(UpdateData.titledeed) ? DBNull.Value : (object)UpdateData.titledeed);
                cmd.Parameters.AddWithValue("@rental_id", UpdateData.rental_id);
                cmd.Parameters.AddWithValue("@uploadtime", DateTime.Now);
                cmd.Parameters.AddWithValue("@check", 0);
                cmd.Parameters.AddWithValue("@tenant", false);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public void DeleteImgData(Guid id)
        {
            string sql = $@"UPDATE RENTAL SET isDelete = 1 WHERE rental_id = @Id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }



        public void InsertHouse_Rental(Rental newData)
        {
            string sql = @"INSERT INTO RENTAL(publisher,genre,pattern,type,title,address,rent,waterfee,electricitybill,adminfee,floor,area,equipmentname,content,img1,img2,img3,img4,img5,titledeed,[check],tenant,uploadtime,isDelete) 
                        VALUES (@publisher,@genre, @pattern, @type, @title, @address, @rent, @waterfee, @electricitybill, @adminfee, @floor, @area, @equipmentname, @content, @img1, @img2, @img3, @img4, @img5, @titledeed, 0, 0, @uploadtime, 0)";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", newData.publisher);
                cmd.Parameters.AddWithValue("@genre", newData.genre);
                cmd.Parameters.AddWithValue("@pattern", newData.pattern);
                cmd.Parameters.AddWithValue("@type", newData.type);
                cmd.Parameters.AddWithValue("@title", newData.title);
                cmd.Parameters.AddWithValue("@address", newData.address);
                cmd.Parameters.AddWithValue("@rent", newData.rent);
                cmd.Parameters.AddWithValue("@waterfee", newData.waterfee);
                cmd.Parameters.AddWithValue("@electricitybill", newData.electricitybill);
                cmd.Parameters.AddWithValue("@adminfee", newData.adminfee);
                cmd.Parameters.AddWithValue("@floor", newData.floor);
                cmd.Parameters.AddWithValue("@area", newData.area);
                cmd.Parameters.AddWithValue("@equipmentname", newData.equipmentname);
                cmd.Parameters.AddWithValue("@content", newData.content);
                cmd.Parameters.AddWithValue("@img1", newData.img1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@img2", newData.img2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@img3", newData.img3 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@img4", newData.img4 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@img5", newData.img5 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@titledeed", newData.titledeed ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@uploadtime", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        #region Image
        public void OldFileCheck(string image)
        {
            if(!string.IsNullOrEmpty(image))
            {
                var filepath = Path.Combine("wwwroot/Image", image);
                string oldFilePath = filepath;
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
        }
        public string? CreateOneImage(IFormFile FormImage)
        {

            if (FormImage != null)
            {
                if(!FormImage.ContentType.StartsWith("image/"))
                {
                    return "檔案格式不正確";
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(FormImage.FileName);

                var filePath = Path.Combine("wwwroot/Image", uniqueFileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    FormImage.CopyTo(stream);
                }

                return uniqueFileName;
            }
            else
            {
                return null;
            }
        }
        #endregion
        #region MembersImg
        public void MembersImgOldFileCheck(string image)
        {
            if(!string.IsNullOrEmpty(image))
            {
                var filepath = Path.Combine("wwwroot/MembersImg", image);
                string oldFilePath = filepath;
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
        }
        public string? MembersImgCreateOneImage(IFormFile FormImage)
        {

            if (FormImage != null)
            {
                if(!FormImage.ContentType.StartsWith("image/"))
                {
                    return "檔案格式不正確";
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(FormImage.FileName);

                var filePath = Path.Combine("wwwroot/MembersImg", uniqueFileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    FormImage.CopyTo(stream);
                }

                return uniqueFileName;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}