using Microsoft.Data.SqlClient;
using api1.Models;
using System.Data;
using System.Collections.Generic;

namespace api1.Service
{
    public class AllDataDBService
    {
        private readonly SqlConnection conn;

        public AllDataDBService(SqlConnection connection)
        {
            conn = connection;
        }

        // public Dictionary<string,int> AllHomegenre()
        // {
        //     Dictionary<string,int> genreConut = new Dictionary<string,int>();
        //     string sql=$@"SELECT genre FROM RENTAL";
        //     try
        //     {
        //         if (conn.State != ConnectionState.Closed)
        //         {
        //             conn.Close();
        //         }
        //         conn.Open();
        //         SqlCommand cmd=new SqlCommand(sql,conn);
        //         SqlDataReader dr = cmd.ExecuteReader();
        //         while(dr.Read())
        //         {
        //             string genre= dr["genre"].ToString();
        //             if(genreConut.ContainsKey(genre))
        //             {
        //                 genreConut[genre]++;
        //             }
        //             else
        //             {
        //                 genreConut[genre]=1;
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception(e.Message.ToString());
        //     }
        //     finally
        //     {
        //         conn.Close();
        //     }
        //     return genreConut;
        // }
        /*這裡使用了 Dictionary<string, int> 來儲存每個類型（genre）及其出現次數。
        在讀取資料時，如果 genreCount 中已經有相同的類型，則次數加一；
        如果沒有，則新增該類型到 genreCount 並設置次數為 1。*/
        public Dictionary<string, int> AllHometype()
        {
            Dictionary<string, int> typeConut = new Dictionary<string, int>();
            string sql = $@"SELECT * FROM RENTAL";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string type = dr["type"].ToString();
                    if (typeConut.ContainsKey(type))
                    {
                        typeConut[type]++;
                    }
                    else
                    {
                        typeConut[type] = 1;
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
        public Dictionary<string, int> AllHomeaddress()
        {
            Dictionary<string, int> addressCount = new Dictionary<string, int>();
            string sql = $@"SELECT * FROM RENTAL";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string address = dr["address"].ToString();
                    string firstThreeChars = address.Substring(0, Math.Min(address.Length, 3));
                    if (addressCount.ContainsKey(firstThreeChars))
                    {
                        addressCount[firstThreeChars]++;
                    }
                    else
                    {
                        addressCount[firstThreeChars] = 1;
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
            string sql = $@"SELECT * FROM REPORT";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
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

        public List<string> Allday(string day)
        {
            List<string> DataList = new List<string>();
            string sql = $@"SELECT {day.ToLower()} FROM BOOKTIME";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
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



        public class GenreData
        {
            public string Genre { get; set; }
            public int Count { get; set; }
        }
        public List<GenreData> AllHomegenre()
        {
            List<GenreData> genreDataList = new List<GenreData>();
            string sql = $@"SELECT genre, COUNT(*) AS Count FROM RENTAL GROUP BY genre";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string genre = dr["genre"].ToString();
                    int count = Convert.ToInt32(dr["Count"]);
                    GenreData genreData = new GenreData { Genre = genre, Count = count };
                    genreDataList.Add(genreData);
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
            return genreDataList;
        }
        public Dictionary<string, List<object>> GetGenreDataForChart()
        {
            List<GenreData> genreDataList = AllHomegenre();

            // 取得 X 軸數據（Genre）
            List<string> xAxisData = genreDataList.Select(data => data.Genre).ToList();

            // 取得 Y 軸數據（Count）
            List<int> yAxisData = genreDataList.Select(data => data.Count).ToList();

            // 建立字典儲存 X 軸和 Y 軸數據
            Dictionary<string, List<object>> chartData = new Dictionary<string, List<object>>();
            chartData.Add("xAxisData", xAxisData.Cast<object>().ToList());
            chartData.Add("yAxisData", yAxisData.Cast<object>().ToList());

            return chartData;
        }
    }
}