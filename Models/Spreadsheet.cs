using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DoWellAdvanced.Models
{
    public class Spreadsheet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titel is verplicht")]
        [Display(Name = "Titel")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "Aangemaakt op")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Zichtbaar")]
        public bool IsVisible { get; set; } = true;

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Navigatie properties
        public virtual ICollection<SpreadsheetTag> SpreadsheetTags { get; set; }
    }
}
