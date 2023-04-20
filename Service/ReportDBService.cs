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
            Report Data=new Report();
            string sql = @"INSERT INTO REPORT(Reported,Reporter,Reason,ReportTime,isCheck,checkTime) 
                        VALUES (@Reported, @Reporter, @Reason, @ReportTime, '0', @checkTime)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Reported", AddData.reported);
                cmd.Parameters.AddWithValue("@Reporter", AddData.reporter);
                cmd.Parameters.AddWithValue("@Reason", AddData.reason);
                cmd.Parameters.AddWithValue("@ReportTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@isCheck", AddData.isCheck);
                cmd.Parameters.AddWithValue("@checkTime", AddData.checkTime);
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