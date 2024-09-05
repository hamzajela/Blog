namespace Blog.Web.Models.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        //One tag can have many posts
        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
