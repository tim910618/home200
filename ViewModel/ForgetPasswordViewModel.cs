using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using api1.Models;

namespace api1.ViewModel
{
    public class ForgetPasswordViewModel
    {
        [DisplayName("帳號:")]
        [Required(ErrorMessage ="請輸入帳號")]
        public string Account{get;set;}
        //public Members newPassword{get;set;}
    }
}