using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace api1.Service
{
    public class ReportDBService
    {
        private readonly SqlConnection conn;

        public ReportDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public void AddReport(Report AddData)
        {
            Report Data = new Report();
            string sql = @"INSERT INTO REPORT(Reported,Reporter,Reason,ReportTime) 
                        VALUES (@Reported, @Reporter, @Reason, @ReportTime)";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Reported", AddData.reported);
                cmd.Parameters.AddWithValue("@Reporter", AddData.reporter);
                cmd.Parameters.AddWithValue("@Reason", AddData.reason);
                cmd.Parameters.AddWithValue("@ReportTime", DateTime.Now);
                //cmd.Parameters.AddWithValue("@isCheck", AddData.isCheck);
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

        public string isBlockAccount(string BlockMember, bool isblcok)
        {
            string sql = string.Empty;
            if (isblcok == false)
            {
                sql = @"Update Members set isBlock='1' where account=@BlockMember";
            }
            else
            {
                sql = @"Update Members set isBlock='0' where account=@BlockMember";
            }
            string message = string.Empty;
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BlockMember", BlockMember);
                cmd.ExecuteNonQuery();

                // 根據isblock的值設定回傳訊息
                if (isblcok == false)
                {
                    message = "停權成功";
                }
                else
                {
                    message = "恢復正常";
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
            return message;
        }

        public List<Report> GetRecord(string account)
        {
            List<Report> DataList = new List<Report>();
            string sql = @"select * from Report m inner join Members d on m.reported = d.Account where d.Account = @Account";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Account", account);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Report Data = new Report();
                        Data.report_Id = (Guid)dr["report_Id"];
                        Data.reported = dr["reported"].ToString();
                        Data.reporter = dr["reporter"].ToString();
                        Data.reason = dr["reason"].ToString();
                        DataList.Add(Data);
                    }
                }
            }
            catch (Exception e)
            {
                // 將例外訊息寫入到日誌或其他記錄檔案中
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }

        public void BlockCancelBooked(string account)
        {
            string sql = $@"update booklist set isDelete=@isDelete where renter = @account or publisher = @account";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@account", account);
                //cmd.Parameters.AddWithValue("@publisher",account);
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
        #region 取得預約清單
        public List<BookList> BookedList(string account)
        {
            List<BookList> DataList = new List<BookList>();
            string sql = $@"
                SELECT booklist.* FROM booklist 
                WHERE (booklist.publisher = @account OR booklist.renter = @account) AND booklist.IsDelete = @IsDelete";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@account", account);
                cmd.Parameters.AddWithValue("@IsDelete", 0);
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
                    Data.isDelete = Convert.ToBoolean(dr["isDelete"]);
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
    }
}