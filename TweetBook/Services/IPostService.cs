using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests.Queries;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter paginationFilter = null);
        Task<bool> CreatePostAsync(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
        Task<List<Tag>> GetAllTagsAsync();
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
        Task<Tag> GetTagByNameAsync(string name);
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string name);
    }
}
