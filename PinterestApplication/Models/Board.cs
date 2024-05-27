namespace PinterestApplication.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<PostBoard>? PostBoards { get; set; }
    }
}
