using System.Reflection;

namespace KITT.Web.App.Client;

public static class Modules
{
    public static IEnumerable<Assembly> AdditionalAssemblies { get; } = [
        typeof(Cms.Web.App._Imports).Assembly,
        typeof(Proposals.Web.App._Imports).Assembly,
    ];
}
