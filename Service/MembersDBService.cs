using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using api1.Models;
using api1.ViewModel;


namespace api1.Service
{
    public class MembersDBService
    {
        //這裡應該是要用看看DI介面、就夠子
        private readonly SqlConnection conn;

        public MembersDBService(SqlConnection connection)
        {
            conn = connection;
        }
        #region 註冊
        #region 新增會員
        public void Register(Members newMember)
        {
            newMember.password = HashPassword(newMember.password);
            string sql = string.Empty;

            if (newMember.identity == 1)
            {
                sql = @"INSERT INTO Members (account,password,name,email,phone,authcode,[identity],score,img,isBlock) 
                    VALUES (@account, @password, @name, @email,@phone, @authcode, '1', @score,@img,'0')";
            }
            else if (newMember.identity == 2)
            {
                sql = @"INSERT INTO Members (account,password,name,email,phone,authcode,[identity],score,img,isBlock) 
                    VALUES (@account, @password, @name, @email,@phone, @authcode, '2', @score,@img,'0')";
            }
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@account", newMember.account);
                cmd.Parameters.AddWithValue("@password", newMember.password);
                cmd.Parameters.AddWithValue("@name", newMember.name);
                cmd.Parameters.AddWithValue("@email", newMember.email);
                cmd.Parameters.AddWithValue("@phone", newMember.phone);
                cmd.Parameters.AddWithValue("@authcode", newMember.authcode);
                cmd.Parameters.AddWithValue("@score", 3.5);
                cmd.Parameters.AddWithValue("@img", newMember.img);
                cmd.Parameters.AddWithValue("@isBlock", 0);
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
        #endregion
        #region Hash密碼
        public string HashPassword(string Password)
        {
            string saltkey = "kdsnkvnakeav123";
            string saltkeyAndPassword = String.Concat(Password, saltkey);
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            //取得密碼轉化成byte資料
            byte[] PassData = Encoding.UTF8.GetBytes(saltkeyAndPassword);
            //取得HASH後byte資料
            byte[] HashData = sha256.ComputeHash(PassData);
            string Hashresult = Convert.ToBase64String(HashData);
            return Hashresult;
        }
        #endregion
        #region 查詢一筆資料
        public Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@"SELECT * FROM MEMBERS WHERE account='{Account}' ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.account = dr["account"].ToString();
                Data.password = dr["password"].ToString();
                Data.name = dr["name"].ToString();
                Data.email = dr["email"].ToString();
                Data.phone = dr["phone"].ToString();
                Data.authcode = dr["authcode"].ToString();
                Data.identity = Convert.ToInt32(dr["identity"]);
                Data.score = Convert.ToDouble(dr["score"]);
                Data.isBlock = Convert.ToBoolean(dr["isBlock"]);
                Data.img = dr["img"].ToString();

                var imgPath = dr["img"].ToString();
                if (!string.IsNullOrEmpty(imgPath))
                {
                    Data.img = $"http://localhost:5190/Image/{imgPath.Replace("\\", "/")}";
                }
            }
            catch (Exception)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion
        #region 帳號註冊重複確認
        public string AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            if (Data == null)
            {
                return "";
            }
            return "已被註冊";
        }
        #endregion
        #region 信箱驗證
        public string EmailValidate(string Account, string AuthCode)
        {
            Members ValidateMember = GetDataByAccount(Account);
            string ValidateStr = string.Empty;
            if (ValidateMember != null)
            {
                if (ValidateMember.authcode == AuthCode)
                {
                    string sql = $@"UPDATE MEMBERS SET authcode='{string.Empty}' WHERE account='{Account}' ";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
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
                    ValidateStr = "帳號信箱驗證成功，現在可以登入";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或在註冊";
                }
            }
            else
            {
                ValidateStr = "查無此帳號，請再重新註冊";
            }
            return ValidateStr;
        }
        #endregion

        #endregion
        #region 登入
        #region 登入確認
        public string LoginCheck(string Account, string Password)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember != null)
            {
                if (LoginMember.isBlock == false)
                {
                    if (string.IsNullOrWhiteSpace(LoginMember.authcode))
                    {
                        if (PasswordCheck(LoginMember, Password))
                        {
                            return "";
                        }
                        else
                        {
                            return "密碼錯誤";
                        }
                    }
                    else
                    {
                        return "尚未驗證，請去EMAIL收驗證信";
                    }
                }
                else
                {
                    return "您已被停權";
                }

            }
            else
            {
                return "無此會員，請去註冊";
            }
        }
        #endregion
        #region 密碼確認
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            bool result = CheckMember.password.Equals(HashPassword(Password));
            return result;
        }
        #endregion
        #region 取得角色
        public string GetRole(string Account)
        {
            string Role = "renter";
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.identity == 1)
            {
                Role = "publisher";
            }
            if (LoginMember.identity == 0)
            {
                Role = "admin";
            }
            return Role;
        }
        #endregion
        #endregion
        #region 修改密碼
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.password = HashPassword(newPassword);
                string sql = $@"UPDATE MEMBERS SET password = '{LoginMember.password}' WHERE account='{Account}' ";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
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
                return "密碼修改完成";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion
        #region 忘記密碼
        public string ForgetPasswordAccount(string Account)
        {
            Members Data = GetDataByAccount(Account);
            if (Data == null)
            {
                return "沒有此帳號";
            }
            return "";
        }
        public void ForgetPassword(string Account, string password)
        {
            Members Data = GetDataByAccount(Account);
            if (Data.account != null)
            {
                Data.password = HashPassword(password);
                string sql = "UPDATE MEMBERS SET password=@Password WHERE account=@Account";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Password", Data.password);
                    cmd.Parameters.AddWithValue("@Account", Data.account);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new System.Exception(e.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public string ForgetPassword()
        {
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I","J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" ,
                            "a", "b", "c", "d", "e", "f", "g", "h", "i","j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" ,
                            "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            string newPassword = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < 10; i++)
            {
                newPassword += Code[rd.Next(Code.Count())];
            }
            return newPassword;
        }


        #endregion

        #region 取得會員清單
        public List<MemberListViewModel> GetDataList()
        {
            List<MemberListViewModel> DataList = new List<MemberListViewModel>();
            string sql = $@"SELECT m.*, 
                        (SELECT COUNT(*) FROM Rental r WHERE r.publisher = m.account) AS RentalCount,
                        (SELECT COUNT(*) FROM Report rp WHERE rp.reported = m.account) AS ReportCount
                        FROM Members m";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MemberListViewModel Data = new MemberListViewModel();
                    Data.account = dr["account"].ToString();
                    Data.password = dr["password"].ToString();
                    Data.name = dr["name"].ToString();
                    Data.email = dr["email"].ToString();
                    Data.phone = dr["phone"].ToString();
                    Data.authcode = dr["authcode"].ToString();
                    Data.identity = Convert.ToInt32(dr["identity"]);
                    Data.score = Convert.ToInt32(dr["score"]);
                    Data.isBlock = Convert.ToBoolean(dr["isBlock"]);
                    Data.rentalCount = dr.GetInt32(dr.GetOrdinal("rentalCount"));
                    Data.reportCount = dr.GetInt32(dr.GetOrdinal("reportCount"));
                    DataList.Add(Data);
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
        #endregion


        #region 修改會員資料
        public void UpdatePro(Members UpdateData)
        {
            string sql = @"UPDATE Members SET name=@name, phone=@phone, img=@img WHERE account=@account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@account", UpdateData.account);
                cmd.Parameters.AddWithValue("@name", UpdateData.name);
                cmd.Parameters.AddWithValue("@phone", UpdateData.phone);
                cmd.Parameters.AddWithValue("@img", UpdateData.img);
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
        #endregion

        /*#region 查詢會員資料陣列 TEST API
        public List<Members> GetDataByAccountList()
        {
            List<Members> DataList = new List<Members>();
            string sql = $@"SELECT * FROM MEMBERS";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read()){
                Members Data=new Members();
                Data.account = dr["account"].ToString();
                Data.password = dr["password"].ToString();
                Data.name = dr["name"].ToString();
                Data.email = dr["email"].ToString();
                Data.phone = dr["phone"].ToString();
                Data.authcode = dr["authcode"].ToString();
                Data.score = Convert.ToInt32(dr["score"]);
                DataList.Add(Data);
                }
            }
            catch(Exception e)
            {
                DataList=null;
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        #endregion*/

    }
}