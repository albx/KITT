using KITT.Cms.Web.Api.Test.Internals;
using KITT.Cms.Web.Models.Settings;
using KITT.Web.Testing.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KITT.Cms.Web.Api.Test;

public class SettingsEndpointsTest : IClassFixture<CmsWebApplicationFactory>
{
    private readonly CmsWebApplicationFactory _factory;

    private readonly static string endpoint = "api/settings";

    public SettingsEndpointsTest(CmsWebApplicationFactory factory)
    {
        _factory = factory;
    }

    #region GetAllSettings tests
    [Fact]
    public async Task GetAllSettings_Should_Return_Unauthorized_If_User_Is_Not_Authenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSettings_Should_Return_Empty_List_If_No_Settings_Are_Available()
    {
        // Arrange
        using var app = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => services.AddTestAuthentication());
        });

        using var client = app.CreateClient();

        // Act
        var response = await client.GetAsync(endpoint);

        // Assert
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<SettingsListModel>();

        Assert.NotNull(model);        
        Assert.Empty(model.Items);
    }
    #endregion

    #region GetSettingsDetails tests
    #endregion

    #region CreateNewSettings tests
    #endregion
}
