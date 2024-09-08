using Bunit;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.Test;

public static class BUnitContextExtensions
{
    public static void SetupFluentUI(this TestContext ctx)
    {
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        ctx.Services.AddFluentUIComponents(options =>
        {
            options.ValidateClassNames = false;
        });

        var module = ctx.JSInterop
            .SetupModule("./_content/Microsoft.FluentUI.AspNetCore.Components/Components/DataGrid/FluentDataGrid.razor.js?v=4.9.3.24205");
        
        module.SetupVoid("init", _ => true);
    }
}
