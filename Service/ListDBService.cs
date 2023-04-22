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
            List<BookList> DataList =new List<BookList>();
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
                    Data.booktime = Convert.ToDateTime(dr["booktime"]);
                    Data.rental_id = (Guid)dr["rental_id"];
                    Data.publisher = dr["booklist_id"].ToString();
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
            return (DataList);
        }
        #endregion

        #region 新增可預約時間

        #endregion
    }
}