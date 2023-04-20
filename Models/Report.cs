namespace api1.Models
{
    public class Report
    {
        public Guid report_Id{get;set;}
        public string reported{get;set;}
        public string? reporter{get;set;}
        
        public string reason{get;set;}
        //public DateTime reportTime{get;set;}
        //public int isCheck{get;set;}
        //public DateTime checkTime{get;set;}
    }
}