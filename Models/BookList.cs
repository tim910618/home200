namespace api1.Models
{
    public class BookList
    {
        public Guid? booklist_id { get; set; }
        public string? renter {get;set;}
        public string? publisher{get;set;}
        public DateOnly bookdate{get;set;}
        public string booktime{get;set;}
        public Guid rental_id{get;set;}
        public bool isCheck{get;set;}
        public bool isDelete{get;set;}
    }
}