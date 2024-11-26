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
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Zichtbaar")]
        public bool IsVisible { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public virtual ICollection<SpreadsheetTag> SpreadsheetTags { get; set; }
    }
}
