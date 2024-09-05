using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository,
            IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked=false;
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();

            if (signInManager.IsSignedIn(User))
            {
                //Get like for this blog for this user, if user has liked this blog before
                var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                //Get the user id
                var userid = userManager.GetUserId(User);
                if (userid != null)
                {
                    var likesFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userid));
                    liked = likesFromUser != null;
                }
            }

            //Get comments for BlogPost
            var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);

            var blogCommentsForView = new List<BlogComment>();
            foreach (var blogComment in blogCommentsDomainModel) {

                blogCommentsForView.Add(new BlogComment
                {
                    Description = blogComment.Description,
                    DateAdded = blogComment.DateAdded,
                    Username = (await userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                });
            }
       

            if (blogPost != null)
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPost.Id);
                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked=liked,
                    Comments= blogCommentsForView

                };


            }

            return View(blogDetailsViewModel);
        }

        [HttpPost]
       public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
             if (signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId = Guid.Parse(userManager.GetUserId(User)),
                    DateAdded=DateTime.Now
                };
               await blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index","Blogs",
                    new {urlHandle=blogDetailsViewModel.UrlHandle});
            }

            return View();
        }
       
    }
}
