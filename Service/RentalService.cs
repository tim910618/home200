/*
using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;

namespace api1.Service
{
    public class RentalService
    {
        private readonly SqlConnection conn;

        public RentalService(SqlConnection connection)
        {
            conn = connection;
        }

        #region 取得單一房屋資料OK
        public Rental GetRentalById(Guid Id)
        {
            Rental Data = new Rental();
            string sql = $@"SELECT m.*,d.* FROM RENTAL m INNER JOIN MEMBERS d ON m.publisher = d.Account WHERE m.rental_id = '{Id}'; ";
            //string sql = $@"SELECT * FROM Rental WHERE rental_id = '{Id}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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
                if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                {
                    Data.img2 = dr["img2"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                {
                    Data.img3 = dr["img3"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                {
                    Data.img4 = dr["img4"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                {
                    Data.img5 = dr["img5"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                {
                    Data.img6 = dr["img6"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                {
                    Data.img7 = dr["img7"].ToString();
                };
                if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                {
                    Data.img8 = dr["img8"].ToString();
                };

                Data.check = Convert.ToInt32(dr["check"]);
                Data.tenant = Convert.ToBoolean(dr["tenant"]);
                Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
                Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
                Data.Member.name = dr["name"].ToString();
            }
            catch (Exception)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if (Data == null)
            {
                return null;
            }
            else
            {
                return Data;
            }
        }
        #endregion

        #region 新增房屋

        public void InsertHouse_Rental(Rental newData)
        {
            string sql = @"INSERT INTO RENTAL(genre,pattern,type,title,address,rent,waterfee,electricitybill,adminfee,floor,area,equipmentname,content,img1,img2,img3,img4,img5,img6,img7,img8,check,tenant,uploadtime,isDelete) 
                        VALUES (@genre, @pattern, @type, @title, @address, @rent, @waterfee, @electricitybill, @adminfee, @floor, @area, @equipmentname, @content, @img1, @img2, @img3, @img4, @img5, @img6, @img7, @img8, 0, 0, @uploadtime, 0)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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
                cmd.Parameters.AddWithValue("@img1", newData.img1);
                cmd.Parameters.AddWithValue("@img2", newData.img2);
                cmd.Parameters.AddWithValue("@img3", newData.img3);
                cmd.Parameters.AddWithValue("@img4", newData.img4);
                cmd.Parameters.AddWithValue("@img5", newData.img5);
                cmd.Parameters.AddWithValue("@img6", newData.img6);
                cmd.Parameters.AddWithValue("@img7", newData.img7);
                cmd.Parameters.AddWithValue("@img8", newData.img8);
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
        #endregion

        #region 修改房屋
        public void UpdateRental(Rental UpdateData)
        {
            string sql = @"UPDATE RENTAL SET genre=@genre, pattern=@pattern, type=@type, title=@title, address=@address, rent=@rent, waterfee=@waterfee, electricitybill=@electricitybill, adminfee=@adminfee, floor=@floor, area=@area,
                equipmentname=@equipmentname, content=@content, img1=@img1, img2=@img2, img3=@img3, img4=@img4, img5=@img5, img6=@img6, img7=@img7, img8=@img8, uploadtime=@uploadtime WHERE rental_id=@rental_id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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
                cmd.Parameters.AddWithValue("@img1", UpdateData.img1);
                cmd.Parameters.AddWithValue("@img2", UpdateData.img2);
                cmd.Parameters.AddWithValue("@img3", UpdateData.img3);
                cmd.Parameters.AddWithValue("@img4", UpdateData.img4);
                cmd.Parameters.AddWithValue("@img5", UpdateData.img5);
                cmd.Parameters.AddWithValue("@img6", UpdateData.img6);
                cmd.Parameters.AddWithValue("@img7", UpdateData.img7);
                cmd.Parameters.AddWithValue("@img8", UpdateData.img8);
                cmd.Parameters.AddWithValue("@rental_id", UpdateData.rental_id);
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
        #endregion

        #region 刪除房屋OK
        public string DeleteRental(Rental Delete)
        {
            string Str = string.Empty;
            if (Delete == null)
            {
                return Str = "此房屋不存在";
            }
            else if (Delete.isDelete == true)
            {
                return Str = "此房屋已被刪除";
            }
            else
            {
                string sql = $@"UPDATE RENTAL SET isDelete = '1' WHERE rental_id = '{Delete.rental_id}';";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
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
                return Str = "刪除成功";
            }
        }
        #endregion

        #region 審核房屋
        public string CheckUpload(Rental CheckData)
        {
            string Str = string.Empty;
            if (CheckData == null)
            {
                return Str = "此房屋不存在";
            }
            else if (CheckData.check == '1')
            {
                return Str = "此房屋已審核";
            }
            else
            {
                string sql = $@"UPDATE RENTAL SET [check] = '1' WHERE rental_id = '{CheckData.rental_id}';";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
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
                return Str = "審核成功";
            }
        }
        #endregion

        #region 取得房屋列表OK
        public List<Rental> GetDataList(string Search, ForPaging Page)
        {
            List<Rental> DataList = new List<Rental>();
            if (!string.IsNullOrWhiteSpace(Search))
            {
                SetMaxPaging(Search, Page);
                DataList = GetAllDataList(Search, Page);
            }
            else
            {
                SetMaxPaging(Page);
                DataList = GetAllDataList(Page);
            }
            return DataList;
        }
        public List<Rental> GetAllDataList(string Search, ForPaging Page)
        {
            List<Rental> DataList = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM
                (
                    SELECT row_number() OVER(ORDER BY Uploadtime) AS sort,* 
                    FROM RENTAL 
                    WHERE (equipmentname LIKE '%'+@Search+'%' or title LIKE '%'+@Search+'%') 
                    and publisher = @Account
                ) m 
                INNER JOIN MEMBERS d ON m.publisher = d.Account 
                WHERE m.sort BETWEEN {(Page.NowPage - 1) * Page.Item + 1} AND {Page.NowPage * Page.Item};"; try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                DataList = null;
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        public List<Rental> GetAllDataList(ForPaging Page)
        {
            List<Rental> DataList = new List<Rental>();
            string sql = $@"SELECT m.*,d.* FROM(SELECT row_number() OVER(ORDER BY Uploadtime) AS sort,* FROM RENTAL where [check]='1' and tenant='0') m INNER JOIN MEMBERS d ON m.publisher = d.Account  WHERE m.sort BETWEEN {(Page.NowPage - 1) * Page.Item + 1} AND {Page.NowPage * Page.Item} and m.publisher = d.Account ; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
                    Data.publisher = dr["publisher"].ToString();
                    Data.title = dr["title"].ToString();
                    Data.rent = Convert.ToInt32(dr["rent"]);
                    Data.img1 = dr["img1"].ToString();
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
                    Data.Member.name = dr["name"].ToString();
                    Data.Member.score = Convert.ToInt32(dr["score"]);
                    DataList.Add(Data);
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
            if (DataList.Count == 0)
            {
                return null;
            }
            else
            {
                return DataList;
            }

        }
        //無搜尋值..
        public void SetMaxPaging(ForPaging Paging)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM Rental; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(string Search, ForPaging Paging)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM Rental WHERE equipmentname LIKE '%{Search}%' ; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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

        #endregion

        #region 取得有無審核房屋列表這-房東OK

        //未審核房屋列表
        public List<Rental> GetUncheckDataList(string account, string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL WHERE  [check] = '0' and tenant = '0'  and publisher = '{account}' ) m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item} and m.publisher = d.Account; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }
        //已審核、下架已承租
        public List<Rental> GetTakeoffDataList(string account, string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL WHERE [check] = '1' and tenant = '1' and publisher = '{account}' ) m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item} and m.publisher = d.Account; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }
        //已審核、上架未承租
        public List<Rental> GetPutOnDataList(string account, string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL WHERE [check] = '1' and tenant = '0' and publisher = '{account}') m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item}  and m.publisher = d.Account; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }
        #endregion

        #region 取得有無審核房屋編號列表-管理員OK

        //查詢房屋資料列表
        public List<Rental> GetAllCheckDataList(string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL) m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                rentals = null;
                //throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }
        //已審核房屋列表
        public List<Rental> GetCheckDataList(string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL WHERE [check] = 1 ) m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
                    //編號、圖片、標題、價錢、位置、更新時間、修改
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.title = dr["title"].ToString();
                    Data.address = dr["address"].ToString();
                    Data.rent = Convert.ToInt32(dr["rent"]);
                    Data.img1 = dr["img1"].ToString();
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                //rentals=null;
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }

        //未審核房屋列表
        public List<Rental> GetUncheckDataList(string search, ForPaging paging)
        {
            List<Rental> rentals = new List<Rental>();
            string sql = $@"SELECT m.*,d.name FROM(SELECT row_number() OVER(ORDER BY rental_id) AS sort,* FROM RENTAL WHERE [check] = 0 ) m INNER JOIN MEMBERS d ON m.publisher = d.Account ";
            if (!string.IsNullOrEmpty(search))
            {
                sql += " WHERE m.title LIKE '%' + @search + '%'";
            }
            sql += $@" AND m.sort BETWEEN {(paging.NowPage - 1) * paging.Item + 1} AND {paging.NowPage * paging.Item}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.Parameters.AddWithValue("@search", search);
                }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Rental Data = new Rental();
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
                    if (!string.IsNullOrWhiteSpace(dr["img2"].ToString()))
                    {
                        Data.img2 = dr["img2"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img3"].ToString()))
                    {
                        Data.img3 = dr["img3"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img4"].ToString()))
                    {
                        Data.img4 = dr["img4"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img5"].ToString()))
                    {
                        Data.img5 = dr["img5"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img6"].ToString()))
                    {
                        Data.img6 = dr["img6"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img7"].ToString()))
                    {
                        Data.img7 = dr["img7"].ToString();
                    };
                    if (!string.IsNullOrWhiteSpace(dr["img8"].ToString()))
                    {
                        Data.img8 = dr["img8"].ToString();
                    };

                    Data.check = Convert.ToInt32(dr["check"]);
                    Data.tenant = Convert.ToBoolean(dr["tenant"]);
                    Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);

                    Data.Member.name = dr["name"].ToString();
                    rentals.Add(Data);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            paging.MaxPage = (int)Math.Ceiling((double)rentals.Count() / paging.Item);
            paging.SetRightPage();
            return rentals;
        }
        #endregion

        #region 下架房屋
        public string Tackoff(Rental Data)
        {
            string Str = string.Empty;
            if (Data == null)
            {
                return Str = "此房屋不存在";
            }
            else if (Data.tenant == true)
            {
                return Str = "此房屋已下架";
            }
            else
            {
                string sql = $@"update RENTAL set tenant='1' where rental_id='{Data.rental_id}' ";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
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
                return Str="下架成功";
            }
        }
        #endregion

        #region 收藏
        public List<Collect> GetFavoriteDataList(string Account, ForPaging Page)
        {
            List<Collect> DataList = new List<Collect>();
            string sql = $@"SELECT m.*, d.*, mem.* FROM (SELECT row_number() OVER (ORDER BY renter) AS sort, * FROM collect) m 
            INNER JOIN Rental d ON m.rental_id = d.rental_id 
            INNER JOIN members mem ON m.renter = mem.account 
            WHERE m.sort BETWEEN {(Page.NowPage - 1) * Page.Item + 1} AND {Page.NowPage * Page.Item} AND renter = '{Account}';";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Collect Data = new Collect();
                    Data.Rental.publisher = dr["publisher"].ToString();
                    Data.Rental.title = dr["title"].ToString();
                    Data.Rental.rent = Convert.ToInt32(dr["rent"]);
                    Data.Rental.img1 = dr["img1"].ToString();
                    Data.Rental.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
                    Data.Member.name = dr["name"].ToString();
                    Data.Member.score = Convert.ToInt32(dr["score"]);
                    DataList.Add(Data);
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
            return DataList;
        }
        //無搜尋值
        public void SetFavoriteMaxPaging(string Account, ForPaging Paging)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM collect where renter='{Account}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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

        //加入收藏
        public void AddFavorite(string Account, Guid Id)
        {
            string checkSql = $@"SELECT COUNT(*) FROM collect WHERE renter='{Account}' AND rental_id='{Id}'";
            string insertSql = $@"INSERT INTO collect(renter, rental_id) VALUES('{Account}', '{Id}')";

            try
            {
                conn.Open();

                // 檢查是否已經有相同的資料存在
                SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                int count = (int)checkCmd.ExecuteScalar();

                // 如果回傳的結果大於0，代表已經有相同的資料存在，不用新增資料
                if (count > 0)
                {
                    return;
                }

                // 如果回傳的結果等於0，代表沒有相同的資料，可以進行新增
                SqlCommand insertCmd = new SqlCommand(insertSql, conn);
                insertCmd.ExecuteNonQuery();
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

        //移除收藏
        public void RemoveFavorite(string Account, Guid Id)
        {
            string sql = $@"Delete from Collect where rental_id='{Id}' and renter='{Account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 判斷有無審核
        public bool CheckRental(Guid Id)
        {
            Rental Data = new Rental();
            string sql = $@"select * from Rental where [check]='1' and rental_id='{Id}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.rental_id = (Guid)dr["rental_id"];
                Data.img1 = dr["img1"].ToString();
                Data.title = dr["title"].ToString();
                Data.rent = Convert.ToInt32(dr["rent"]);
                Data.address = dr["address"].ToString();
                Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if (Data == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 判斷有無出租
        public bool TenantRental(Guid Id)
        {
            Rental Data = new Rental();
            string sql = $@"select * from Rental where tenant='1' and rental_id='{Id}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                //編號、圖片、標題、價錢、位置、更新時間、(修改上架刪除)
                Data.rental_id = (Guid)dr["rental_id"];
                Data.img1 = dr["img1"].ToString();
                Data.title = dr["title"].ToString();
                Data.rent = Convert.ToInt32(dr["rent"]);
                Data.address = dr["address"].ToString();
                Data.uploadtime = Convert.ToDateTime(dr["uploadtime"]);
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            if (Data == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}

*/