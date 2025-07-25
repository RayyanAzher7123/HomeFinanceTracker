using System.ComponentModel.DataAnnotations;

namespace HomeFinance.web.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation property
        public List<Expense> Expenses { get; set; }
    }
}
