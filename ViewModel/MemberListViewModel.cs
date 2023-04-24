using api1.Models;

namespace api1.ViewModel
{
    public class MemberListViewModel
    {
        public Guid? members_id { get; set; }

        public string? account { get; set; }
        public string? password { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? authcode { get; set; }
        public int? identity { get; set; }
        public int? score { get; set; }
        public string? img { get; set; }
        public bool? isBlock{get;set;}
        public int rentalCount { get; set; }
        public int reportCount { get; set; }
    }
}