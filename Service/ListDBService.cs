using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;

namespace api1.Service
{
    public class ListDBService
    {
        private readonly SqlConnection conn;

        public ListDBService(SqlConnection connection)
        {
            conn = connection;
        }

        #region 取得預約清單
        public List<BookList> GetBookTime(string account)
        {
            List<BookList> DataList = new List<BookList>();
            string sql = $@"select * from booklist where renter=@renter or publisher=@publisher and IsDelete=@isDelete";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@publisher", account);
                cmd.Parameters.AddWithValue("@IsDelete", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BookList Data = new BookList();
                    Data.renter = dr["renter"].ToString();
                    Data.publisher = dr["publisher"].ToString();
                    Data.bookdate = DateOnly.FromDateTime(Convert.ToDateTime(dr["bookdate"]));
                    Data.booktime = dr["booktime"].ToString();
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.booklist_id = (Guid)dr["booklist_id"];
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
            return (DataList);
        }
        #endregion

        #region 新增可預約時間
        public void Addbooking(BookList Data)
        {
            string sql = @"insert into booklist(booklist_id,renter,publisher,bookdate,booktime,rental_id,isDelete) values (@booklist_id,@renter,@publisher,@bookdate,@booktime,@rental_id,@isDelete)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", Data.renter);
                cmd.Parameters.AddWithValue("@publisher", Data.publisher);
                cmd.Parameters.AddWithValue("@bookdate", Data.bookdate);
                cmd.Parameters.AddWithValue("@booktime", Data.booktime);
                cmd.Parameters.AddWithValue("@rental_id", Data.rental_id);
                cmd.Parameters.AddWithValue("@isDelete", '0');
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
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@isDelete", '0');
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
            string sql = @"SELECT COUNT(*) FROM booklist WHERE publisher=@publisher AND bookdate=@bookdate AND booktime=@booktime AND renter != @renter and IsDelete=@IsDelete" ;
            try
            {
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


    }
}