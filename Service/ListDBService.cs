using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;

namespace api1.Service
{
    public class ListDBService
    {
        private readonly SqlConnection conn;

        public ListDBService(SqlConnection connection)
        {
            conn = connection;
        }

        #region 取得已預約清單
        public List<GetBookListViewModel> GetBookTime1(string account)
        {
            List<GetBookListViewModel> DataList = new List<GetBookListViewModel>();
            string sql = $@"
                SELECT booklist.*, rental.address, rental.title, rental.img1 FROM booklist 
                INNER JOIN rental ON booklist.rental_id = rental.rental_id 
                WHERE (booklist.renter=@renter OR booklist.publisher=@publisher) AND booklist.IsDelete=@isDelete AND booklist.isCheck=@isCheck";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@publisher", account);
                cmd.Parameters.AddWithValue("@IsDelete", '0');
                cmd.Parameters.AddWithValue("@isCheck", '1');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    GetBookListViewModel Data = new GetBookListViewModel();
                    Data.renter = dr["renter"].ToString();
                    Data.publisher = dr["publisher"].ToString();
                    Data.bookdate = DateOnly.FromDateTime(Convert.ToDateTime(dr["bookdate"]));
                    Data.booktime = dr["booktime"].ToString();
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.booklist_id = (Guid)dr["booklist_id"];
                    Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
                    Data.Title = dr["title"].ToString();
                    Data.Address = dr["address"].ToString();
                    Data.img1 = dr["img1"].ToString();
                    Data.isCheck = Convert.ToBoolean(dr["isCheck"]);
                    DataList.Add(Data);

                    var imgPath = dr["img1"].ToString();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        if (!imgPath.Contains("http://"))
                        {
                            imgPath = $"http://localhost:5190/Image/{imgPath.Replace("\\", "/")}";
                        }
                        Data.img1 = imgPath;
                    }
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
            return (DataList);
        }
        #endregion

        #region 取得未預約清單
        public List<GetBookListViewModel> GetBookTime0(string account)
        {
            List<GetBookListViewModel> DataList = new List<GetBookListViewModel>();
            string sql = $@"
                SELECT booklist.*, rental.address, rental.title, rental.img1 FROM booklist 
                INNER JOIN rental ON booklist.rental_id = rental.rental_id 
                WHERE (booklist.renter=@renter OR booklist.publisher=@publisher) AND booklist.IsDelete=@isDelete AND booklist.isCheck=@isCheck";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@publisher", account);
                cmd.Parameters.AddWithValue("@IsDelete", '0');
                cmd.Parameters.AddWithValue("@isCheck", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    GetBookListViewModel Data = new GetBookListViewModel();
                    Data.renter = dr["renter"].ToString();
                    Data.publisher = dr["publisher"].ToString();
                    Data.bookdate = DateOnly.FromDateTime(Convert.ToDateTime(dr["bookdate"]));
                    Data.booktime = dr["booktime"].ToString();
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.booklist_id = (Guid)dr["booklist_id"];
                    Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
                    Data.Title = dr["title"].ToString();
                    Data.Address = dr["address"].ToString();
                    Data.img1 = dr["img1"].ToString();
                    Data.isCheck = Convert.ToBoolean(dr["isCheck"]);
                    DataList.Add(Data);

                    var imgPath = dr["img1"].ToString();
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        if (!imgPath.Contains("http://"))
                        {
                            imgPath = $"http://localhost:5190/Image/{imgPath.Replace("\\", "/")}";
                        }
                        Data.img1 = imgPath;
                    }
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
            return (DataList);
        }
        #endregion

        #region 新增可預約時間
        public void Addbooking(BookList Data)
        {
            string sql = @"insert into booklist(booklist_id, renter, publisher, bookdate, booktime, rental_id, isCheck, isDelete) 
                    values (@booklist_id, @renter, @publisher, @bookdate, @booktime, @rental_id, @isCheck, @isDelete)";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", Data.renter);
                cmd.Parameters.AddWithValue("@publisher", Data.publisher);
                cmd.Parameters.AddWithValue("@bookdate", Data.bookdate);
                cmd.Parameters.AddWithValue("@booktime", Data.booktime);
                cmd.Parameters.AddWithValue("@rental_id", Data.rental_id);
                cmd.Parameters.AddWithValue("@isDelete", '0');
                cmd.Parameters.AddWithValue("@isCheck", '0');
                // 產生新的 GUID 並加入到 BookList 物件中
                Data.booklist_id = Guid.NewGuid();
                // 將 GUID 加入到 SQL INSERT 語句中
                cmd.Parameters.AddWithValue("@booklist_id", Data.booklist_id);
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

        #region 取消預約
        public void CancelBooking(Guid id)
        {
            string sql = @"update booklist set isDelete=@isDelete where booklist_id = @id";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@isDelete", '1');
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

        #region 確認預約
        public bool IsBooked(DateOnly bookdate, string booktime, string publisher, string renter)
        {
            bool isBooked = false;
            string sql = @"SELECT COUNT(*) FROM booklist WHERE publisher=@publisher AND bookdate=@bookdate AND booktime=@booktime AND renter != @renter and IsDelete=@IsDelete";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@bookdate", bookdate);
                cmd.Parameters.AddWithValue("@booktime", booktime);
                cmd.Parameters.AddWithValue("@renter", renter);
                cmd.Parameters.AddWithValue("@isDelete", '0');
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    isBooked = true;
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
            return isBooked;
        }

        #endregion

        #region 判斷時間
        public bool CheckBooked(string renter, string publisher, DateOnly date, string time)
        {
            string sql = @"select * from booklist where renter=@renter and publisher=@publisher and bookdate=@date and booktime=@time and isDelete=@IsDelete";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", renter);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@IsDelete", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    return true;
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
            return false;
        }


        //時間有無交集
        public bool IsTimeOverlap(string booktime, DateOnly bookdate, string account)
        {
            string[] timeParts = booktime.Split('-');
            TimeSpan book_startTime = TimeSpan.Parse(timeParts[0]);
            TimeSpan book_endTime = TimeSpan.Parse(timeParts[1]);

            string sql = @"select booktime from booklist where (renter=@renter or publisher=@publisher) and bookdate=@date and IsDelete=@isDelete";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@publisher", account);
                cmd.Parameters.AddWithValue("@date", bookdate);
                cmd.Parameters.AddWithValue("@isDelete", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                List<string> bookedTimes = new List<string>();
                while (dr.Read())
                {
                    bookedTimes.Add(dr.GetString(0));
                }

                foreach (string bookedTime in bookedTimes)
                {
                    string[] bookedTimeParts = bookedTime.Split('-');
                    TimeSpan booked_startTime = TimeSpan.Parse(bookedTimeParts[0]);
                    TimeSpan booked_endTime = TimeSpan.Parse(bookedTimeParts[1]);
                    if (book_startTime < booked_endTime && book_endTime > booked_startTime)
                    {
                        return true;
                    }
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
            return false;
        }

        #endregion

        #region 取得單筆資料
        public BookList GetBookTimeById(Guid Id)
        {
            BookList Data = new BookList();
            string sql = $@"SELECT * FROM booklist WHERE booklist_id=@id";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Id);
                //cmd.Parameters.AddWithValue("@IsDelete", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.renter = dr["renter"].ToString();
                    Data.publisher = dr["publisher"].ToString();
                    Data.bookdate = DateOnly.FromDateTime(Convert.ToDateTime(dr["bookdate"]));
                    Data.booktime = dr["booktime"].ToString();
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.booklist_id = (Guid)dr["booklist_id"];
                    Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
                }
            }
            catch
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data);
        }
        #endregion

        #region 房東確認是否預約
        public string CheckBooking(Guid Book_Id, string state)
        {
            if (state == "1")
            {
                string sql = $@"update booklist set isCheck=@state where booklist_Id=@Id";
                try
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@state", '1');
                    cmd.Parameters.AddWithValue("@Id", Book_Id);
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
                return "預約成功";
            }
            else
            {
                string sql = $@"update booklist set isCheck=@state,isDelete=@Delete where booklist_Id=@Id";
                try
                {
                    if (conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@state", '1');
                    cmd.Parameters.AddWithValue("@Delete", '1');
                    cmd.Parameters.AddWithValue("@Id", Book_Id);
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
            return "取消預約成功";
        }
        #endregion
    }
}