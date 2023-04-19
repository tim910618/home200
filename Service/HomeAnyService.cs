using Microsoft.Data.SqlClient;
using api1.Models;

namespace api1.Service
{
    public class HomeAnyDBService
    {
        private readonly SqlConnection conn;

        public HomeAnyDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public List<Guid> GetIdList(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by rental_id desc) AS sort,* FROM RENTAL WHERE tenant = 1 ) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
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

    }
}