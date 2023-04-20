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
                cmd.Parameters.AddWithValue("@isCheck", AddData.isCheck);
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

        public void BlockAccount(Members BlockData)
        {
            string sql = @"Update Members set isBlcok=@pattern where";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@check", 1);
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