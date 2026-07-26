using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NameVi { get; set; } = string.Empty;

        [StringLength(50)]
        public string IconName { get; set; } = "folder";

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
