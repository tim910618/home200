using System.Data;
using api1.Models;
using Microsoft.Data.SqlClient;

namespace api1.Service
{
    public class ScoreService
    {
        private readonly SqlConnection conn;
        public ScoreService (SqlConnection connection)
        {
            conn=connection;
        }

        public void CalculateScore(Members Data,double Score)
        {
            double a=0.3;
            Data.score = Math.Round((double)(a * Score + (1 - a) * Data.score), 2);
            if (Data.score > 5) Data.score = 5;
            if (Data.score < 0) Data.score = 0;
            
            string sql=$@"UPDATE Members SET score=@score WHERE Account=@account ;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@score", Data.score);
                cmd.Parameters.AddWithValue("@account", Data.account);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
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