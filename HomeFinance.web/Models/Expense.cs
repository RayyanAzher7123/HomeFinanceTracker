using System.ComponentModel.DataAnnotations;

namespace HomeFinance.web.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //Foreign Key
        public int? StoreId { get; set; }

        //Navigate
        public Store? Store { get; set; }

        public string? BillImagePath { get; set; }



    }
}