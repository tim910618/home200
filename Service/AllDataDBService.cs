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

        public class GenreData
        {
            public string Genre { get; set; }
            public int Count { get; set; }
        }
        public class TypeData
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }
        public class AddressData
        {
            public string Address { get; set; }
            public int Count { get; set; }
        }

        #region 取得預約清單陣列
        public List<BookList> GetBookTimeById(string Account,DateOnly date)
        {
            List<BookList> Data = new List<BookList>();
            string sql = $@"SELECT * FROM booklist WHERE isDelete=@IsDelete and IsCheck=@IsCheck and bookdate=@Date and publisher=@Account";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IsDelete", '0');
                cmd.Parameters.AddWithValue("@IsCheck", '1');
                cmd.Parameters.AddWithValue("@Account", Account);
                cmd.Parameters.AddWithValue("@Date", date);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BookList Data2=new BookList();
                    Data2.renter = dr["renter"].ToString();
                    Data2.publisher = dr["publisher"].ToString();
                    Data2.bookdate = DateOnly.FromDateTime(Convert.ToDateTime(dr["bookdate"]));
                    Data2.booktime = dr["booktime"].ToString();
                    Data2.rental_id = (Guid)dr["rental_id"];
                    Data2.booklist_id = (Guid)dr["booklist_id"];
                    Data2.isDelete = Convert.ToBoolean(dr["isDelete"]);
                    Data2.isCheck = Convert.ToBoolean(dr["isCheck"]);
                    Data.Add(Data2);
                }
            }
            catch
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data);
        }
        #endregion

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
            chartData.Add("labels", xAxisData.Cast<object>().ToList());
            chartData.Add("data", yAxisData.Cast<object>().ToList());

            return chartData;
        }
        
        public List<TypeData> AllHometype()
        {
            List<TypeData> typeDataList = new List<TypeData>();
            string sql = $@"SELECT type, COUNT(*) AS Count FROM RENTAL GROUP BY type";
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
                    int count = Convert.ToInt32(dr["Count"]);
                    TypeData typeData = new TypeData { Type = type, Count = count };
                    typeDataList.Add(typeData);
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
            return typeDataList;
        }
        public Dictionary<string, List<object>> GetTypeDataForChart()
        {
            List<TypeData> typeDataList = AllHometype();

            List<string> xAxisData = typeDataList.Select(data => data.Type).ToList();
            List<int> yAxisData = typeDataList.Select(data => data.Count).ToList();
            Dictionary<string, List<object>> chartData = new Dictionary<string, List<object>>();
            chartData.Add("labels", xAxisData.Cast<object>().ToList());
            chartData.Add("data", yAxisData.Cast<object>().ToList());

            return chartData;
        }
        public List<AddressData> AllHomeaddress()
        {
            List<AddressData> addressDataList = new List<AddressData>();
            string sql = $@"SELECT address, COUNT(*) AS Count FROM RENTAL GROUP BY address";
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
                    int count = Convert.ToInt32(dr["Count"]);
                    if (address.Length > 3)
                    {
                        address = address.Substring(0, 3); // 只保留前三個字符
                    }
                    AddressData addressData = new AddressData { Address = address, Count = count };
                    addressDataList.Add(addressData);
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
            return addressDataList;
        }
        public Dictionary<string, List<object>> GetAddressDataForChart()
        {
            List<AddressData> addressDataList = AllHomeaddress();
            List<string> xAxisData = addressDataList.Select(data => data.Address).ToList();
            List<int> yAxisData = addressDataList.Select(data => data.Count).ToList();
            Dictionary<string, List<object>> chartData = new Dictionary<string, List<object>>();
            chartData.Add("labels", xAxisData.Cast<object>().ToList());
            chartData.Add("data", yAxisData.Cast<object>().ToList());

            return chartData;
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
        public class LoginData
        {
            //身分
            public int Identity{get;set;}
            //登入次數
            public int LoginCount{get;set;}
        }
        public List<LoginData> AllHomeLogin() 
        {
            List<LoginData> loginDataList=new List<LoginData>();
            int identity1Count=0;
            int identity2Count=0;
            try
            {
                int totalPersonCount = 0;
                string sql=$@"SELECT [identity], COUNT(*) AS LoginCount, COUNT(DISTINCT Account) AS PersonCount FROM Login_Record WHERE [identity]=1 GROUP BY [identity]" ;
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd=new SqlCommand(sql,conn);
                SqlDataReader dr=cmd.ExecuteReader();
                while(dr.Read())
                {
                    int identity = Convert.ToInt32(dr["identity"]); 
                    int loginCount=Convert.ToInt32(dr["LoginCount"]);
                    int personCount = Convert.ToInt32(dr["PersonCount"]);

                    identity1Count = loginCount;
                    totalPersonCount +=personCount;

                }
                loginDataList.Add(new LoginData
                {
                    Identity=1,
                    LoginCount=identity1Count
                });
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            

            try
            {
                int totalPersonCount = 0;
                string sql=$@"SELECT [identity], COUNT(*) AS LoginCount, COUNT(DISTINCT Account) AS PersonCount FROM Login_Record WHERE [identity]=2 GROUP BY [identity]" ;
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd=new SqlCommand(sql,conn);
                SqlDataReader dr=cmd.ExecuteReader();
                while(dr.Read())
                {
                    int identity = Convert.ToInt32(dr["identity"]); 
                    int loginCount=Convert.ToInt32(dr["LoginCount"]);
                    int personCount = Convert.ToInt32(dr["PersonCount"]);

                    identity2Count = loginCount;
                    totalPersonCount +=personCount;

                    
                }
                loginDataList.Add(new LoginData
                {
                    Identity=2,
                    LoginCount=identity2Count
                });
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return loginDataList;
        }
    }
}



////////////////////////////////////////////////////////////
/*public Dictionary<string,int> AllHomegenre()
        {
            Dictionary<string,int> genreConut = new Dictionary<string,int>();
            string sql=$@"SELECT genre FROM RENTAL";
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
                    string genre= dr["genre"].ToString();
                    if(genreConut.ContainsKey(genre))
                    {
                        genreConut[genre]++;
                    }
                    else
                    {
                        genreConut[genre]=1;
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
            return genreConut;
        }*/
        /*這裡使用了 Dictionary<string, int> 來儲存每個類型（genre）及其出現次數。
        在讀取資料時，如果 genreCount 中已經有相同的類型，則次數加一；
        如果沒有，則新增該類型到 genreCount 並設置次數為 1。*/
        /*public Dictionary<string, int> AllHometype()
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
        }*/
        /*public Dictionary<string, int> AllHomeaddress()
        {
            List<GenreData> genreDataList = AllHomegenre();

            // 取得 X 軸數據（Genre）
            List<string> xAxisData = genreDataList.Select(data => data.Genre).ToList();

            // 取得 Y 軸數據（Count）
            List<int> yAxisData = genreDataList.Select(data => data.Count).ToList();

            // 建立字典儲存 X 軸和 Y 軸數據
            Dictionary<string, List<object>> chartData = new Dictionary<string, List<object>>();
            chartData.Add("labels", xAxisData.Cast<object>().ToList());
            chartData.Add("data", yAxisData.Cast<object>().ToList());

            return chartData;
        }
    }
}
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
        }*/