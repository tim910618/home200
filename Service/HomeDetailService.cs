using Microsoft.Data.SqlClient;
using api1.Models;

namespace api1.Service
{
    public class HomeDetailDBService
    {
        private readonly SqlConnection conn;

        public HomeDetailDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public List<Guid> GetIdList(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by rental_id desc) AS sort,* FROM RENTAL WHERE tenant = 0 AND isDelete = 0 AND [check] = 0 ) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    IdList.Add(Guid.Parse(dr["rental_id"].ToString()));
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
            return IdList;
        }
        public void SetMaxPaging(ForPaging Paging)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM RENTAL; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) 
                {
                    Row++;
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
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.Item));
            Paging.SetRightPage();
        }

        public void CheckHome(Rental CheckData)
        {
            string sql=$@"UPDATE RENTAL SET [check]=@check,tenant=@tenant WHERE rental_id = @Id;";  
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (CheckData.check == 0)
                {
                    cmd.Parameters.AddWithValue("@check", 0);
                    cmd.Parameters.AddWithValue("@tenant", false);
                }
                else if (CheckData.check == 1)
                {
                    cmd.Parameters.AddWithValue("@check", 1);
                    cmd.Parameters.AddWithValue("@tenant", true);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@check",2);
                    cmd.Parameters.AddWithValue("@tenant", false);
                }
                cmd.Parameters.AddWithValue("@Id", CheckData.rental_id);
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