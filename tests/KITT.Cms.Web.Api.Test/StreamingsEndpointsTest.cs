using KITT.Cms.Web.Api.Test.Internals;

namespace KITT.Cms.Web.Api.Test;

internal class StreamingsEndpointsTest : IClassFixture<CmsWebApplicationFactory>
{
    private readonly CmsWebApplicationFactory _factory;

    public StreamingsEndpointsTest(CmsWebApplicationFactory factory)
    {
        _factory = factory;
    }
}
