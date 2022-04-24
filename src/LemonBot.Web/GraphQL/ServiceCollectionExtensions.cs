using HotChocolate.Types.Pagination;
using LemonBot.Web.GraphQL.Queries;

namespace LemonBot.Web.GraphQL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKittGrahpQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddFiltering()
            .AddSorting()
            .SetPagingOptions(new PagingOptions
            {
                IncludeTotalCount = true
            })
            .AddQueryType<StreamingsQuery>();

        return services;
    }
}
