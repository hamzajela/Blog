﻿using Blog.Web.Models.ViewModels;

namespace Blog.Web.Repositories
{
    public interface IBlogPostCommentRepository
    {
        Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment);
        Task <IEnumerable<BlogPostComment>> GetCommentsByBlogIdAsync(Guid blogPostId);  
    }
}
