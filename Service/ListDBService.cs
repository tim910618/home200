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
            string sql = $@"select * from booklist where renter=@renter or publisher=@publisher";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@publisher", account);
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
            string sql = @"insert into booklist(renter,publisher,bookdate,booktime,rental_id) values (@renter,@publisher,@bookdate,@booktime,@rental_id)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", Data.renter);
                cmd.Parameters.AddWithValue("@publisher", Data.publisher);
                cmd.Parameters.AddWithValue("@bookdate", Data.bookdate);
                cmd.Parameters.AddWithValue("@booktime", Data.booktime);
                cmd.Parameters.AddWithValue("@rental_id", Data.rental_id);
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
            string sql = @"delete from booklist where booklist_id = @id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
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
        public bool IsBooked(DateOnly bookdate, string booktime, string publisher)
        {
            bool isBooked = false;
            string sql = @"SELECT COUNT(*) FROM booklist WHERE publisher=@publisher AND bookdate=@bookdate AND booktime=@booktime";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@bookdate", bookdate);
                cmd.Parameters.AddWithValue("@booktime", booktime);
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

        #region 
        public bool CheckBooked(string account, DateOnly date, string time)
        {
            string sql = @"select * from booklist where renter=@renter and bookdate=@date and booktime=@time";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@time", time);
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

            string sql = @"select booktime from booklist where renter=@renter and bookdate=@date";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", account);
                cmd.Parameters.AddWithValue("@date", bookdate);
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