using System.ComponentModel.DataAnnotations;

namespace PinterestApplication.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category name required")]
        public string Name { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }

    }
}
