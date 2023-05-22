using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;

namespace api1.Service
{
    public class TimeDBService
    {
        private readonly SqlConnection conn;

        public TimeDBService(SqlConnection connection)
        {
            conn = connection;
        }

        #region 設定可預約時間
        public void SetBookTime(string account, BookTime bookTime)
        {
            // 將可預約時間轉成陣列
            string[] monTime = !string.IsNullOrEmpty(bookTime.mon) ? bookTime.mon.Split(';') : new string[0];
            string[] tueTime = !string.IsNullOrEmpty(bookTime.tue) ? bookTime.tue.Split(';') : new string[0];
            string[] wedTime = !string.IsNullOrEmpty(bookTime.wed) ? bookTime.wed.Split(';') : new string[0];
            string[] thuTime = !string.IsNullOrEmpty(bookTime.thu) ? bookTime.thu.Split(';') : new string[0];
            string[] friTime = !string.IsNullOrEmpty(bookTime.fri) ? bookTime.fri.Split(';') : new string[0];
            string[] satTime = !string.IsNullOrEmpty(bookTime.sat) ? bookTime.sat.Split(';') : new string[0];
            string[] sunTime = !string.IsNullOrEmpty(bookTime.sun) ? bookTime.sun.Split(';') : new string[0];

            // 驗證時間格式是否正確
            bool isValidFormat = true;
            string errorMessage = "以下時間格式有誤：\n";
            foreach (string time in monTime.Concat(tueTime).Concat(wedTime).Concat(thuTime).Concat(friTime).Concat(satTime).Concat(sunTime))
            {
                if (!Regex.IsMatch(time, @"^\d{2}:\d{2}-\d{2}:\d{2}$")) //格式09:00-12:00驗證
                {
                    isValidFormat = false;
                    errorMessage += time + "\n";
                }
            }

            if (!isValidFormat)
            {
                throw new ArgumentException(errorMessage);
            }

            // 新增至資料庫
            string sql = string.Empty;
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd;
                if (bookTime.booktime_id != Guid.Empty)
                {
                    // 修改資料庫中的預約時間
                    sql = "UPDATE BookTime SET monday = @monday, tuesday = @tuesday, wednesday = @wednesday, thursday = @thursday, friday = @friday, saturday = @saturday, sunday = @sunday WHERE publisher = @publisher AND booktime_id = @id ";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@publisher", account);
                    cmd.Parameters.AddWithValue("@id", bookTime.booktime_id);
                }
                else
                {
                    // 新增至資料庫
                    sql = "INSERT INTO BookTime (publisher, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (@publisher, @monday, @tuesday, @wednesday, @thursday, @friday, @saturday, @sunday)";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@publisher", account);
                }
                cmd.Parameters.AddWithValue("@monday", string.Join(";", monTime));
                cmd.Parameters.AddWithValue("@tuesday", string.Join(";", tueTime));
                cmd.Parameters.AddWithValue("@wednesday", string.Join(";", wedTime));
                cmd.Parameters.AddWithValue("@thursday", string.Join(";", thuTime));
                cmd.Parameters.AddWithValue("@friday", string.Join(";", friTime));
                cmd.Parameters.AddWithValue("@saturday", string.Join(";", satTime));
                cmd.Parameters.AddWithValue("@sunday", string.Join(";", sunTime));
                cmd.ExecuteNonQuery();

                // // 刪除資料庫中的資料
                // if (bookTime.booktime_id != null && bookTime.booktime_id.Any())
                // {
                //     string deleteSql = $"DELETE FROM BookTime WHERE id IN ({string.Join(",", bookTime.booktime_id)})";

                //     try
                //     {
                //         conn.Open();
                //         SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
                //         deleteCmd.ExecuteNonQuery();
                //     }
                //     catch (Exception e)
                //     {
                //         throw new Exception(e.Message.ToString());
                //     }
                //     finally
                //     {
                //         conn.Close();
                //     }
                // }
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

        #region 取得可預約時間表
        public BookTime GetBookTime(string account)
        {
            BookTime Data = new BookTime();
            string sql = $@"select * from BookTime where publisher=@publisher";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", account);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.booktime_id = (Guid)dr["booktime_id"];
                Data.publisher = dr["publisher"].ToString();
                Data.mon = dr["monday"].ToString();
                Data.tue = dr["tuesday"].ToString();
                Data.wed = dr["wednesday"].ToString();
                Data.thu = dr["thursday"].ToString();
                Data.fri = dr["friday"].ToString();
                Data.sat = dr["saturday"].ToString();
                Data.sun = dr["sunday"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return (Data);
        }
        #endregion

        #region 取得單一天的預約時間
        public BookTime GetBookOfDay(string account)
        {
            BookTime Data = new BookTime();
            string sql = $@"select * from BookTime where publisher=@publisher";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", account);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.booktime_id = (Guid)dr["booktime_id"];
                Data.publisher = dr["publisher"].ToString();
                Data.mon = dr["monday"].ToString();
                Data.tue = dr["tuesday"].ToString();
                Data.wed = dr["wednesday"].ToString();
                Data.thu = dr["thursday"].ToString();
                Data.fri = dr["friday"].ToString();
                Data.sat = dr["saturday"].ToString();
                Data.sun = dr["sunday"].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return (Data);
        }

        // 取得當天可預約的時段
        private string GetAvailableTimes(string account, DateTime date)
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            // 先取得屋主帳號的BookTime資料
            BookTime bookTime = GetBookOfDay(account);

            // 取得當日是星期幾
            DayOfWeek dayOfWeek = date.DayOfWeek;

            // 根據星期幾取得可預約時間的字串
            string availableTimes = "";
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    availableTimes = bookTime.mon;
                    break;
                case DayOfWeek.Tuesday:
                    availableTimes = bookTime.tue;
                    break;
                case DayOfWeek.Wednesday:
                    availableTimes = bookTime.wed;
                    break;
                case DayOfWeek.Thursday:
                    availableTimes = bookTime.thu;
                    break;
                case DayOfWeek.Friday:
                    availableTimes = bookTime.fri;
                    break;
                case DayOfWeek.Saturday:
                    availableTimes = bookTime.sat;
                    break;
                case DayOfWeek.Sunday:
                    availableTimes = bookTime.sun;
                    break;
                default:
                    throw new ArgumentException("請選擇正確的日期格式");
            }

            // 回傳可預約時間的字串
            return availableTimes;
        }

        public SpecialTime GetSpecialTime(string publisher, DateTime date)
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            SpecialTime data = null;
            string sql = "SELECT * FROM SpecialTime WHERE publisher=@publisher AND Date=@date";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@date", date);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    data = new SpecialTime();
                    data.special_id = (Guid)dr["special_id"];
                    data.publisher = dr["publisher"].ToString();
                    data.date = (DateTime)dr["Date"];
                    data.oldtime = dr["oldtime"].ToString();
                    data.newtime = dr["newtime"].ToString();
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
            return data;
        }


        #endregion
        #region 取得可預約預約時間Id
        public Guid GetBookTime_Id(string account)
        {
            BookTime Data = new BookTime();
            string sql = $@"select booktime_id from BookTime where publisher=@publisher";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", account);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return (Guid)result;
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

            return Guid.Empty;
        }
        #endregion

        #region 取得特別時段時間Id
        public Guid GetSpecialTime_Id(string account)
        {
            BookTime Data = new BookTime();
            string sql = $@"select Special_id from SpecialTime where publisher=@publisher";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", account);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return (Guid)result;
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

            return Guid.Empty;
        }
        #endregion

        #region 新增特殊時間
        public void AddSpecialTime(SpecialTime Data)
        {
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                // 檢查資料庫中是否已有該日期的特殊時間設定
                string selectSql = "SELECT COUNT(*) FROM SpecialTime WHERE publisher = @publisher AND date = @date";
                SqlCommand selectCmd = new SqlCommand(selectSql, conn);
                selectCmd.Parameters.AddWithValue("@publisher", Data.publisher);
                selectCmd.Parameters.AddWithValue("@date", Data.date);
                int count = (int)selectCmd.ExecuteScalar();

                if (count > 0)
                {
                    // 資料庫已有該日期的特殊時間設定，使用 UPDATE 進行更新
                    string updateSql = "UPDATE SpecialTime SET oldtime = @oldtime, newtime = @newtime WHERE publisher = @publisher AND date = @date";
                    SqlCommand updateCmd = new SqlCommand(updateSql, conn);
                    updateCmd.Parameters.AddWithValue("@publisher", Data.publisher);
                    updateCmd.Parameters.AddWithValue("@date", Data.date);
                    updateCmd.Parameters.AddWithValue("@oldtime", Data.oldtime);
                    updateCmd.Parameters.AddWithValue("@newtime", Data.newtime);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // 資料庫中尚未有該日期的特殊時間設定，使用 INSERT 進行新增
                    string insertSql = "INSERT INTO SpecialTime (publisher, date, oldtime, newtime) VALUES (@publisher, @date, @oldtime, @newtime)";
                    SqlCommand insertCmd = new SqlCommand(insertSql, conn);
                    insertCmd.Parameters.AddWithValue("@publisher", Data.publisher);
                    insertCmd.Parameters.AddWithValue("@date", Data.date);
                    insertCmd.Parameters.AddWithValue("@oldtime", Data.oldtime);
                    insertCmd.Parameters.AddWithValue("@newtime", Data.newtime);
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 取得房東已被預約陣列 
        public string[] GetBookedTimes(string account, DateTime date, string[] availableTimes)
        {
            List<string> bookedTimes = new List<string>();
            string sql = $"SELECT booktime FROM booklist WHERE isDelete=@isDelete and publisher = @account AND bookdate = @date AND booktime IN ({string.Join(",", availableTimes.Select(t => $"'{t}'"))})";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@account", account);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@isDelete", '0');
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string time = reader.GetString(0);
                    bookedTimes.Add(time);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return bookedTimes.ToArray();
        }

        #endregion

        public bool IsReserved(DateTime date, string newTime, string publisher)
        {
            // 取得所有已預約的時間
            List<BookList> bookedTimes = new List<BookList>();
            string sql = @"select * from booklist where publisher=@publisher and isDelete=@isDelete";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@isDelete", '0');
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BookList Data = new BookList();
                    Data.renter = dr["renter"].ToString();
                    Data.publisher = dr["publisher"].ToString();
                    Data.bookdate = DateOnly.FromDateTime(DateTime.Parse(dr["bookdate"].ToString()));
                    Data.booktime = dr["booktime"].ToString();
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.booklist_id = (Guid)dr["booklist_id"];
                    bookedTimes.Add(Data);
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

            // 將新的時間段轉換成起始時間和結束時間
            string[] newTimeParts = newTime.Split(";");
            foreach (BookList bookedTime in bookedTimes)
            {
                // 如果日期不相同，則跳過這個已預約的時間段
                if (bookedTime.bookdate.ToDateTime(new TimeOnly(0, 0, 0)) != date.Date)
                {
                    continue;
                }

                // 將已預約的時間段轉換成起始時間和結束時間
                string[] bookedTimeParts = bookedTime.booktime.Split("-");
                TimeOnly bookedStartTime = TimeOnly.Parse(bookedTimeParts[0]);
                TimeOnly bookedEndTime = TimeOnly.Parse(bookedTimeParts[1]);

                // 檢查兩個時間段是否有重疊
                if (bookedStartTime < bookedEndTime && bookedStartTime < bookedEndTime)
                {
                    // 有重疊，表示這個時間段已經被預約了
                    return true;
                }
            }


            // 如果沒有任何重疊，則表示這個時間段還沒有被預約
            return false;
        }
    }
}