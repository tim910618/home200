namespace api1.Models
{
    public class Collect
    {
        public Guid collect_id { get; set; }
        public string renter { get; set; }
        public Guid rental_id { get; set; }
        public Members Member { get; set; }=new Members();
        public Rental Rental{get;set;}=new Rental();
    }
}