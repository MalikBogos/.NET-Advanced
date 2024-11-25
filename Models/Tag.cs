using System.ComponentModel.DataAnnotations;

namespace DoWellAdvanced.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Naam")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Display(Name = "Zichtbaar")]
        public bool IsVisible { get; set; } = true;

        // Navigatie property
        public virtual ICollection<SpreadsheetTag> SpreadsheetTags { get; set; }
    }
}
