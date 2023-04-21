namespace api1.Models
{
    public class BookTime
    {
        public int BookTimeId { get; set; }
        public string Publisher { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; }
    }
}