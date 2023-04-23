namespace api1.Models
{
    public class SpecialTime
    {
        public Guid? special_id { get; set; }
        public string? publisher{get;set;}
        public DateTime date { get; set; }
        public string? oldtime { get; set; }
        public string newtime{get;set;}
    }
}