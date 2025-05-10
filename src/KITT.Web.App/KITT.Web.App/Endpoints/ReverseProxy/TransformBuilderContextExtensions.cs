using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace KITT.Web.App.Endpoints.ReverseProxy;

public static class TransformBuilderContextExtensions
{
    public static void ConfigureWithTargetPath(this TransformBuilderContext transformBuilder, string targetPath)
    {
        transformBuilder.AddPathRouteValues(new(targetPath));
    }
}
