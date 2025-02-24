﻿using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogDbContext blogDbContext;

        public BlogPostRepository(BlogDbContext blogDbContext)
        {
            this.blogDbContext = blogDbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await blogDbContext.AddAsync(blogPost);
            await blogDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlog=await blogDbContext.Blog_Posts.FindAsync(id);
           if (existingBlog != null)
            {
                blogDbContext.Blog_Posts.Remove(existingBlog);  
                await blogDbContext.SaveChangesAsync();
                return existingBlog;

            }
           return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
          return await blogDbContext.Blog_Posts.Include(x=>x.Tags).ToListAsync(); 
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await blogDbContext.Blog_Posts.Include(x=>x.Tags).FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
           return await blogDbContext.Blog_Posts.Include(x=>x.Tags).
             FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);

        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
          var existingBlog=  await blogDbContext.Blog_Posts.Include(x=>x.Tags)
                .FirstOrDefaultAsync(x=>x.Id==blogPost.Id);
            if (existingBlog != null) 
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.Author = blogPost.Author;
                existingBlog.PageTitle=blogPost.PageTitle;
                existingBlog.ShortDescription=blogPost.ShortDescription;    
                existingBlog.FeaturedImageUrl=blogPost.FeaturedImageUrl; 
                existingBlog.Content=blogPost.Content;  
                existingBlog.UrlHandle=blogPost.UrlHandle;
                existingBlog.Visible=blogPost.Visible;  
                existingBlog.PublishedDate=blogPost.PublishedDate;      
                existingBlog.Tags = blogPost.Tags;

                await blogDbContext.SaveChangesAsync();
                return existingBlog;
            }
            return null;
        }
    }
}
