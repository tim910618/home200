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

        public Dictionary<string, int> AllHomegenre()
        {
            Dictionary<string, int> genreCount = new Dictionary<string, int>();
            //List<string> DataList = new List<string>();
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
                    string genre = dr["genre"].ToString();
                    if (genreCount.ContainsKey(genre))
                    {
                        genreCount[genre]++;
                    }
                    else
                    {
                        genreCount[genre] = 1;
                    }
                    //DataList.Add(dr["genre"].ToString());
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
            return genreCount;
        }
        /*這裡使用了 Dictionary<string, int> 來儲存每個類型（genre）及其出現次數。
        在讀取資料時，如果 genreCount 中已經有相同的類型，則次數加一；
        如果沒有，則新增該類型到 genreCount 並設置次數為 1。*/
        public Dictionary<string,int> AllHometype()
        {
            Dictionary<string,int> typeConut = new Dictionary<string,int>();
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
                    string type= dr["type"].ToString();
                    if(typeConut.ContainsKey(type))
                    {
                        typeConut[type]++;
                    }
                    else
                    {
                        typeConut[type]=1;
                    }
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
            return typeConut;
        }
        public Dictionary<string,int> AllHomeaddress()
        {
            Dictionary<string,int> addressCount = new Dictionary<string,int>();
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
                    if(addressCount.ContainsKey(firstThreeChars))
                    {
                        addressCount[firstThreeChars]++;
                    }
                    else
                    {
                        addressCount[firstThreeChars]=1;
                    }
                    //DataList.Add(firstThreeChars);
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
            return addressCount;
        }

        public List<string> AllReason()
        {
            List<string> DataList = new List<string>();
            string sql=$@"SELECT * FROM REPORT";
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
                    DataList.Add(dr["Reason"].ToString());
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

        public List<string> Allmon(string day)
        {
            List<string> DataList = new List<string>();
            string sql=$@"SELECT {day.ToLower()} FROM BOOKTIME";
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
                    DataList.Add(dr.GetString(0));
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