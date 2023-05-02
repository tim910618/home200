using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace api1.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string Account{get;set;}
        // [DisplayName("舊密碼:")]
        // [Required(ErrorMessage ="請輸入舊密碼")]
        public string Password{get;set;}
        // [DisplayName("新密碼:")]
        // [Required(ErrorMessage ="請輸入新密碼")]
        public string NewPassword{get;set;}
        // [DisplayName("新密碼確認:")]
        // [Required(ErrorMessage ="請輸入新密碼確認")]
        // [Compare("NewPassword",ErrorMessage ="密碼輸入不一致")]
        public string CheckNewPassword{get;set;}
    }
}