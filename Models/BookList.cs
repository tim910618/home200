namespace api1.Models
{
    public class BookList
    {
        public string renter {get;set;}
        public string publisher{get;set;}
        public DateTime booktime{get;set;}
        public Guid rental_id{get;set;}
    }
}