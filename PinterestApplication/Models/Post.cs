using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinterestApplication.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title required")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Post description required")]
        public string Description { get; set; }

        public byte[] Image {  get; set; }

        [Required(ErrorMessage = "Category required")]
        public int CategoryId { get; set; }
        public string? UserId { get; set; }
 
        public virtual Category? Category { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public string? Keywords { get; set; }
        public string? Url{ get; set; }

        public virtual ICollection<Like>? Likes { get; set; }
        public virtual ICollection<PostBoard>? PostBoards { get; set; }
        
        public virtual ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Categories { get; set; }
        
    }
}
