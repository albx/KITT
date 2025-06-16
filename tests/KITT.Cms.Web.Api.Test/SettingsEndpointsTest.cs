using KITT.Cms.Web.Api.Test.Internals;
using KITT.Cms.Web.Models.Settings;
using KITT.Web.Testing.Authentication;
using KITT.Web.Testing.DependencyInjection;
using KITT.Web.Testing.Security;
using System.IdentityModel.Tokens.Jwt;

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

    [Fact(Skip = "Must be fixed")]
    public async Task GetAllSettings_Should_Return_Empty_List_If_No_Settings_Are_Available()
    {
        // Arrange
        using var app = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(
                services => services.ConfigureTestJwtOptions(CmsWebApplicationFactory.TenantId, CmsWebApplicationFactory.AppId));
        });

        var token = TestJwtTokenProvider.JwtSecurityTokenHandler.WriteToken(
            new JwtSecurityToken(
                $"https://sts.windows.net/{CmsWebApplicationFactory.TenantId}/",
                $"api://{CmsWebApplicationFactory.AppId}",
                new TestClaimsProvider().Claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: TestJwtTokenProvider.SigningCredentials));

        using var client = app.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

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
