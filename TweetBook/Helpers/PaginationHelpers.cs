using System;
using System.Collections.Generic;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Contracts.V1.Requests.Queries;
using TweetBook.Domain;
using TweetBook.Contracts.V1;
using TweetBook.Services;
using System.Linq;

namespace TweetBook.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginationResponse<T>(IUriService uriService, PaginationFilter pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1 && uriService != null ?
                uriService.GetAllPostsUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize))?.ToString()
                : null;
            
            var previousPage = pagination.PageNumber - 1 >= 1 && uriService != null ?
                uriService.GetAllPostsUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize))?.ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage
            };
        }
    }
}
