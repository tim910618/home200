namespace api1.Models
{
    public class GetBookListViewModel
    {
        public Guid? booklist_id { get; set; }
        public string? renter { get; set; }
        public string? publisher { get; set; }
        public DateOnly bookdate { get; set; }
        public string booktime { get; set; }
        public Guid rental_id { get; set; }
        public bool isDelete { get; set; }
        public bool isCheck { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string img1 { get; set; }
    }
}