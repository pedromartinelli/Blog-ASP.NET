namespace Blog.Models
{
    public class Category
    {
        public Category(string name, string slug)
        {
            Id = 0;
            Name = name;
            Slug = slug;
            Posts = null;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public IList<Post>? Posts { get; set; } = new List<Post>();
    }
}