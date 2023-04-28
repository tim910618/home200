using api1.Models;

namespace api1.ViewModel
{
    public class CheckViewModel
    {
        public Guid Id{get;set;}
        public int Type{get;set;}
        public string? Reason { get; set; }
    }
}