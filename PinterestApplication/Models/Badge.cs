namespace PinterestApplication.Models
{
    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

    }
}
