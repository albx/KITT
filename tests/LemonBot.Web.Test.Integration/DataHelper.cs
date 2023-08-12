using KITT.Core.Models;
using KITT.Core.Persistence;

namespace LemonBot.Web.Test.Integration;

internal static class DataHelper
{
    public static void PrepareDataForTest(IServiceCollection services, Action<KittDbContext>? customDataPreparation = null)
    {
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var shouldSave = false;

        var context = scopedServices.GetRequiredService<KittDbContext>();
        if (!context.Settings.Any())
        {
            context.Settings.Add(Settings.CreateNew(TestAuthenticationHandler.UserId, "albx87"));
            shouldSave = true;
        }

        if (context.Streamings.Any())
        {
            context.Streamings.RemoveRange(context.Streamings);
            shouldSave = true;
        }

        if (shouldSave)
        {
            context.SaveChanges();
        }

        customDataPreparation?.Invoke(context);
    }
}
