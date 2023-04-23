namespace api1.Models
{
    public class CheckBooked
    {
        public DateOnly bookdate { get; set; }
        public string? booktime { get; set; }
        public string? publisher { get; set; }
    }
}