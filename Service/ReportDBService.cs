using Microsoft.Data.SqlClient;
using api1.Models;
using Microsoft.AspNetCore.Mvc;

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
            string sql = @"INSERT INTO REPORT(Reported,Reporter,Reason,ReportTime,isCheck) 
                        VALUES (@Reported, @Reporter, @Reason, @ReportTime, '0')";
            try
            {
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


    }
}