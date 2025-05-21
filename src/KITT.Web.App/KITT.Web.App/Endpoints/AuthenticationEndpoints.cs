using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KITT.Web.App.Endpoints;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder builder)
    {
        var authenticationGroup = builder.MapGroup("/authentication");

        authenticationGroup
            .MapGet("login", Login)
            .WithName(nameof(Login))
            .AllowAnonymous();

        authenticationGroup
            .MapPost("logout", Logout)
            .WithName(nameof(Logout));

        return builder;
    }

    private static async Task<SignOutHttpResult> Logout(
        HttpContext httpContext,
        [FromForm]string? returnUrl)
    {
        await Task.CompletedTask;
        return TypedResults.SignOut(
            BuildAuthenticationProperties(returnUrl, httpContext),
            [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
    }

    private static async Task<ChallengeHttpResult> Login(
        HttpContext httpContext,
        string? returnUrl)
    {
        await Task.CompletedTask;
        return TypedResults.Challenge(BuildAuthenticationProperties(returnUrl, httpContext));
    }

    private static AuthenticationProperties BuildAuthenticationProperties(string? returnUrl, HttpContext httpContext)
    {
        string pathBase = httpContext.Request.PathBase.Value ?? "/";

        // Prevent open redirects.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = pathBase;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}
