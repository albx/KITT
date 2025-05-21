using Microsoft.Identity.Web;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace KITT.Web.App.Endpoints.ReverseProxy;

public static class TransformBuilderContextExtensions
{
    public static void ConfigureWithTargetPath(
        this TransformBuilderContext transformBuilder, 
        string targetPath,
        Func<IConfiguration, IEnumerable<string>> scopesResolver)
    {
        transformBuilder.AddPathRouteValues(new(targetPath));
        transformBuilder.AddRequestTransform(async transformContext =>
        {
            var tokenAcquisition = transformContext.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
            var configuration = transformContext.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var scopes = scopesResolver.Invoke(configuration) 
                ?? throw new IOException("No downstream API scopes!");

            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);

            transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);
        });
    }
}
