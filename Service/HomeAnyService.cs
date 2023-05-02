using Microsoft.Data.SqlClient;
using api1.Models;
using api1.ViewModel;
using System.Text;
using System.Data;

namespace api1.Service
{
    public class HomeAnyDBService
    {
        private readonly SqlConnection conn;

        public HomeAnyDBService(SqlConnection connection)
        {
            conn = connection;
        }

        public List<Guid> GetIdListSeePublisher(ForPaging Paging,string publisher)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE publisher=@publisher AND tenant = 1 AND isDelete = 0) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@publisher", publisher);
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

        public List<Guid> GetIdListDown(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
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
        public List<Guid> GetIdListUp(ForPaging Paging)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime asc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
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

        public List<Guid> GetIdListDown(ForPaging Paging,AnySearchViewModel search)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>(); 
            //string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by rental_id desc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                StringBuilder sqlBuilder =new StringBuilder();
                var addressBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime desc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE 1=1");
                if(!string.IsNullOrEmpty(search.county) || !string.IsNullOrEmpty(search.township) || !string.IsNullOrEmpty(search.street))
                {
                    if (!string.IsNullOrEmpty(search.county))
                    {
                        addressBuilder.Append(search.county);
                    }
                    if (!string.IsNullOrEmpty(search.township))
                    {
                        addressBuilder.Append(search.township);
                    }
                    if (!string.IsNullOrEmpty(search.street))
                    {
                        addressBuilder.Append(search.street);
                    }
                    sqlBuilder.Append(" AND m.address LIKE @address");
                }
                if(search.rent1.HasValue && search.rent2.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent BETWEEN @rent1 AND @rent2");
                }
                else if(search.rent1.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent >= @rent1");
                }
                else if(search.rent2.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent <= @rent2");
                }
//-------------------------------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(search.genre))
                {
                    sqlBuilder.Append(" AND m.genre LIKE @genre");
                }
                if (!string.IsNullOrEmpty(search.pattern))
                {
                    sqlBuilder.Append(" AND m.pattern LIKE @pattern");
                }
                if (!string.IsNullOrEmpty(search.type))
                {
                    sqlBuilder.Append(" AND m.type LIKE @type");
                }
                if (!string.IsNullOrEmpty(search.equipmentname))
                {
                    sqlBuilder.Append(" AND m.equipmentname LIKE @equipmentname");
                }
                string sql=$@"{sqlBuilder.ToString()} AND m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            SqlCommand cmd = new SqlCommand(sql, conn);
                if(!string.IsNullOrEmpty(search.county) || !string.IsNullOrEmpty(search.township) || !string.IsNullOrEmpty(search.street))
                {
                    cmd.Parameters.AddWithValue("@address",$"%{addressBuilder.ToString()}%");
                }
                if(search.rent1.HasValue && search.rent2.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent1", search.rent1.Value);
                    cmd.Parameters.AddWithValue("@rent2", search.rent2.Value);
                }
                else if(search.rent1.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent1", search.rent1.Value);
                }
                else if(search.rent2.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent2", search.rent2.Value);
                }
                
                if (!string.IsNullOrEmpty(search.genre))
                {
                    cmd.Parameters.AddWithValue("@genre", $"%{search.genre}%");
                }
                if (!string.IsNullOrEmpty(search.pattern))
                {
                    cmd.Parameters.AddWithValue("@pattern", $"%{search.pattern}%");
                }
                if (!string.IsNullOrEmpty(search.type))
                {
                    cmd.Parameters.AddWithValue("@type", $"%{search.type}%");
                }
                if (!string.IsNullOrEmpty(search.equipmentname))
                {
                    cmd.Parameters.AddWithValue("@equipmentname", $"%{search.equipmentname}%");
                }

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
        public List<Guid> GetIdListUp(ForPaging Paging,AnySearchViewModel search)
        {
            SetMaxPaging(Paging);
            List<Guid> IdList = new List<Guid>(); 
            //string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by rental_id desc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                StringBuilder sqlBuilder =new StringBuilder();
                var addressBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT rental_id FROM (SELECT row_number() OVER(order by uploadtime asc) AS sort,* FROM RENTAL WHERE tenant = 1 AND isDelete = 0) m WHERE 1=1");
                if(!string.IsNullOrEmpty(search.county) || !string.IsNullOrEmpty(search.township) || !string.IsNullOrEmpty(search.street))
                {
                    if (!string.IsNullOrEmpty(search.county))
                    {
                        addressBuilder.Append(search.county);
                    }
                    if (!string.IsNullOrEmpty(search.township))
                    {
                        addressBuilder.Append(search.township);
                    }
                    if (!string.IsNullOrEmpty(search.street))
                    {
                        addressBuilder.Append(search.street);
                    }
                    sqlBuilder.Append(" AND m.address LIKE @address");
                }
                if(search.rent1.HasValue && search.rent2.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent BETWEEN @rent1 AND @rent2");
                }
                else if(search.rent1.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent >= @rent1");
                }
                else if(search.rent2.HasValue)
                {
                    sqlBuilder.Append(" AND m.rent <= @rent2");
                }
//-------------------------------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(search.genre))
                {
                    sqlBuilder.Append(" AND m.genre LIKE @genre");
                }
                if (!string.IsNullOrEmpty(search.pattern))
                {
                    sqlBuilder.Append(" AND m.pattern LIKE @pattern");
                }
                if (!string.IsNullOrEmpty(search.type))
                {
                    sqlBuilder.Append(" AND m.type LIKE @type");
                }
                if (!string.IsNullOrEmpty(search.equipmentname))
                {
                    sqlBuilder.Append(" AND m.equipmentname LIKE @equipmentname");
                }
                string sql=$@"{sqlBuilder.ToString()} AND m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            SqlCommand cmd = new SqlCommand(sql, conn);
                if(!string.IsNullOrEmpty(search.county) || !string.IsNullOrEmpty(search.township) || !string.IsNullOrEmpty(search.street))
                {
                    cmd.Parameters.AddWithValue("@address",$"%{addressBuilder.ToString()}%");
                }
                if(search.rent1.HasValue && search.rent2.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent1", search.rent1.Value);
                    cmd.Parameters.AddWithValue("@rent2", search.rent2.Value);
                }
                else if(search.rent1.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent1", search.rent1.Value);
                }
                else if(search.rent2.HasValue)
                {
                    cmd.Parameters.AddWithValue("@rent2", search.rent2.Value);
                }
                
                if (!string.IsNullOrEmpty(search.genre))
                {
                    cmd.Parameters.AddWithValue("@genre", $"%{search.genre}%");
                }
                if (!string.IsNullOrEmpty(search.pattern))
                {
                    cmd.Parameters.AddWithValue("@pattern", $"%{search.pattern}%");
                }
                if (!string.IsNullOrEmpty(search.type))
                {
                    cmd.Parameters.AddWithValue("@type", $"%{search.type}%");
                }
                if (!string.IsNullOrEmpty(search.equipmentname))
                {
                    cmd.Parameters.AddWithValue("@equipmentname", $"%{search.equipmentname}%");
                }

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

        //讀取全部蒐藏資料
        public List<Guid> GetIdListAllCollect(ForPaging Paging,string renter)
        {
            CollectSetMaxPaging(Paging,renter);
            List<Guid> IdList = new List<Guid>();
            string sql = $@" SELECT rental_id FROM (SELECT row_number() OVER(order by collect_id desc) AS sort,* FROM COLLECT WHERE renter = @renter) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.Item + 1} AND {Paging.NowPage * Paging.Item}; ";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", renter);
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
        public void CollectSetMaxPaging(ForPaging Paging,string renter)
        {
            int Row = 0;
            string sql = $@" SELECT * FROM COLLECT WHERE renter=@renter; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", renter);
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
        //新增蒐藏
        public void InsertCollect(Collect newData)
        {
            string sql = @"INSERT INTO COLLECT(renter,rental_id) VALUES (@renter,@rental_id) ;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter",newData.renter);
                cmd.Parameters.AddWithValue("@rental_id", newData.rental_id);
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
        //取消蒐藏
        public void RemoveCollect(string renter,Guid rental_id)
        {
            string sql = @"SELECT collect_id FROM COLLECT WHERE renter = @renter AND rental_id = @rental_id;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@renter", renter);
                cmd.Parameters.AddWithValue("@rental_id", rental_id);
                object collectIdObj = cmd.ExecuteScalar();
                if (collectIdObj == null)
                {
                    throw new Exception("找不到收藏紀錄");
                }
                var collectId = (Guid)collectIdObj;

                // 刪除收藏紀錄
                string deleteSql = @"DELETE FROM COLLECT WHERE collect_id=@collect_id";
                SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
                deleteCmd.Parameters.AddWithValue("@collect_id", collectId);
                deleteCmd.ExecuteNonQuery();
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
        //檢查是否已蒐藏
        public bool CheckCollect(string renter,Guid rental_id)
        {
            string sql=$@"SELECT COUNT(*) FROM COLLECT WHERE renter=@renter AND rental_id=@rental_id ;";
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd =new SqlCommand(sql,conn);
                cmd.Parameters.AddWithValue("@renter",renter);
                cmd.Parameters.AddWithValue("@rental_id",rental_id);
                int count = (int)cmd.ExecuteScalar();
                if(count>0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw new Exception (e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
    }
}