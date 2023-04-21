using api1.Models;

namespace api1.ViewModel
{
    public class AnySearchViewModel
    {
        //縣市+鄉鎮市區+街路
        public string? county{get;set;}
        public string? township{get;set;}
        public string? street{get;set;}
////////////////////////////////////////////////////////
        public int? rent1 { get; set; }
        public int? rent2 { get; set; }

        public string? genre { get; set; }
        public string? pattern { get; set; }
        public string? type { get; set; }
        public string? equipmentname { get; set; }
        //public DateTime? uploadtime { get; set; }
    }
}