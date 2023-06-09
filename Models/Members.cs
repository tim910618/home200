
using System.Text.Json.Serialization;
namespace api1.Models
{

        public class Members{
        public string? account { get; set; }
        [JsonIgnore]
        public string? password { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? authcode { get; set; }
        public int? identity { get; set; }
        public double? score { get; set; }
        public string? img{get;set;}
        public bool? isBlock{get;set;}
}
}