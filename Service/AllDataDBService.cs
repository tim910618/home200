using Microsoft.Data.SqlClient;
using api1.Models;
using System.Data;

namespace api1.Service
{
    public class AllDataDBService
    {
        private readonly SqlConnection conn;

        public AllDataDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public List<string> AllHomegenre()
        {
            List<string> DataList = new List<string>();
            string sql=$@"SELECT * FROM RENTAL";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd=new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    DataList.Add(dr["genre"].ToString());
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
            return DataList;
        }
        public List<string> AllHometype()
        {
            List<string> DataList = new List<string>();
            string sql=$@"SELECT * FROM RENTAL";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd=new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    DataList.Add(dr["type"].ToString());
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
            return DataList;
        }
        public List<string> AllHomeaddress()
        {
            List<string> DataList = new List<string>();
            string sql=$@"SELECT * FROM RENTAL";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd=new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    string address = dr["address"].ToString();
                    string firstThreeChars = address.Substring(0, Math.Min(address.Length, 3));
                    DataList.Add(firstThreeChars);
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
            return DataList;
        }
    }
}