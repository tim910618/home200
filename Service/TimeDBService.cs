using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;

namespace api1.Service
{
    public class TimeDBService
    {
        private readonly SqlConnection conn;

        public TimeDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public void AddBookTime(BookTime bookTime)
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
            string sql = "INSERT INTO BookTime (publisher, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (@publisher, @monday, @tuesday, @wednesday, @thursday, @friday, @saturday, @sunday)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", bookTime.publisher);
                cmd.Parameters.AddWithValue("@monday", string.Join(";", monTime));
                cmd.Parameters.AddWithValue("@tuesday", string.Join(";", tueTime));
                cmd.Parameters.AddWithValue("@wednesday", string.Join(";", wedTime));
                cmd.Parameters.AddWithValue("@thursday", string.Join(";", thuTime));
                cmd.Parameters.AddWithValue("@friday", string.Join(";", friTime));
                cmd.Parameters.AddWithValue("@saturday", string.Join(";", satTime));
                cmd.Parameters.AddWithValue("@sunday", string.Join(";", sunTime));
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
    }
}