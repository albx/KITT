using KITT.Services;
using KITT.Web.App.Endpoints.ReverseProxy;

namespace KITT.Web.App.Endpoints;

internal static class CmsEndpoints
{
    extension(IEndpointRouteBuilder builder)
    {
        public IEndpointRouteBuilder MapCmsEndpoints()
        {
            builder.MapForwarder(
                "/api/cms/streamings", 
                $"https+http://{ServiceNames.CmsApi}", 
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/streamings", 
                    GetScopes))
                .RequireAuthorization();

            builder.MapForwarder(
                "/api/cms/streamings/{id}",
                $"https+http://{ServiceNames.CmsApi}", 
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/streamings/{id}",
                    GetScopes))
                .RequireAuthorization();

            builder.MapForwarder(
                "/api/cms/streamings/import",
                $"https+http://{ServiceNames.CmsApi}", 
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/streamings/import",
                    GetScopes))
                .RequireAuthorization();

            builder.MapForwarder(
                "/api/cms/streamings/stats", 
                $"https+http://{ServiceNames.CmsApi}", 
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/streamings/stats", 
                    GetScopes))
                .RequireAuthorization();

            builder.MapForwarder(
                "/api/cms/settings/channels", 
                $"https+http://{ServiceNames.CmsApi}", 
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/settings/channels", 
                    GetScopes))
                .RequireAuthorization();

            builder.MapForwarder(
                "/api/cms/settings/channels/{id}",
                $"https+http://{ServiceNames.CmsApi}",
                transformBuilder => transformBuilder.ConfigureWithTargetPath(
                    "/api/settings/channels/{id}",
                    GetScopes))
                .RequireAuthorization();

            return builder;
        }
    }
    
    private static IEnumerable<string> GetScopes(IConfiguration configuration)
        => [
            $"api://{configuration["Identity:Cms:AppId"]}/Cms.Read",
            $"api://{configuration["Identity:Cms:AppId"]}/Cms.Write",
        ];
}
