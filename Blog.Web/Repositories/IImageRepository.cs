using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Blog.Web.Repositories
{
    public interface IImageRepository
    {
        Task<string> UploadAsync (IFormFile file);

    }

}
