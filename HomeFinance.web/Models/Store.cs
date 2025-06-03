using System.ComponentModel.DataAnnotations;

namespace HomeFinance.web.Models
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }
    

        [Required]
        public string StoreName { get; set; }
    }
}
