using api1.Models;

namespace api1.ViewModel
{
    public class AnySearchViewModel
    {
        public string? genre { get; set; }
        public string? pattern { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public string? address { get; set; }
        public int? rent { get; set; }
        public string? floor { get; set; }
        public float? area { get; set; }
        public string? equipmentname { get; set; }
        public string? content { get; set; }
    }
}