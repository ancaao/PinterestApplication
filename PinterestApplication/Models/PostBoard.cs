using System.ComponentModel.DataAnnotations.Schema;

namespace PinterestApplication.Models
{
    public class PostBoard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // cheie primara compusa (Id, ArticleId, BookmarkId)
        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? BoardId { get; set; }

        public virtual Post? Post { get; set; }
        public virtual Board? Board { get; set; }

    }
}
