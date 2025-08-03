using Swashbuckle.AspNetCore.Filters;
using TweetBook.Contracts.V1.Responses;

namespace TweetBook.SwaggerExamples.Responses
{
    public class TagResponseExample : IExamplesProvider<TagResponse>
    {
        public TagResponse GetExamples()
        {
            return new TagResponse
            {
                Name = "New tag"
            };
        }
    }
}
