using Swashbuckle.AspNetCore.Filters;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.SwaggerExamples.Requests
{
    public class CreateTagRequestExample : IExamplesProvider<CreateTagRequest>
    {
        public CreateTagRequest GetExamples()
        {
            return new CreateTagRequest
            {
                TagName = "New tag",
            };
        }
    }
}
