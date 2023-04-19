namespace api1.Models
{
        public class Rental
        {
        public Guid rental_id { get; set; }
        public string? publisher { get; set; }
        public string genre { get; set; }
        public string pattern { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public int rent { get; set; }
        public float waterfee { get; set; }
        public float electricitybill { get; set; }
        public int adminfee { get; set; }
        public string floor { get; set; }
        public float area { get; set; }
        public string equipmentname { get; set; }
        public string content { get; set; }

        public string? img1 { get; set; }
        public IFormFile? img1_1 { get; set; }
        public string? img2 { get; set; }
        public string? img3 { get; set; }
        public string? img4 { get; set; }
        public string? img5 { get; set; }

        public IFormFile? img1_2 { get; set; }
        public IFormFile? img1_3 { get; set; }
        public IFormFile? img1_4 { get; set; }
        public IFormFile? img1_5 { get; set; }
        public int check { get; set; }
        public bool tenant { get; set; }
        public DateTime uploadtime { get; set; }
        public bool isDelete{get;set;}

        public Members Member { get; set; } = new Members();
        }
}