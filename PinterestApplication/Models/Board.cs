using System.ComponentModel.DataAnnotations;


namespace PinterestApplication.Models
{
    public class Board
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Board name required")]
        public string Name { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<PostBoard>? PostBoards { get; set; }
    }
}
