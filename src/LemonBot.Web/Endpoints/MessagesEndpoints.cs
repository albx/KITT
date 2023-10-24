using KITT.Web.Models.Messages;
using LemonBot.Web.Endpoints.Services;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Endpoints;

public static class MessagesEndpoints
{
    public static IEndpointRouteBuilder MapMessagesEndpoints(this IEndpointRouteBuilder builder)
    {
        var messagesGroup = builder
            .MapGroup("api/messages")
            .RequireAuthorization()
            .WithParameterValidation();

        messagesGroup
            .MapPost("", SendMessage)
            .WithName(nameof(SendMessage))
            .WithOpenApi();

        return builder;
    }

    private static async Task<IResult> SendMessage(
        MessagesEndpointsServices services,
        [FromBody] SendMessageModel model)
    {
        await services.SendMessageAsync(model);
        return Results.Accepted();
    }
}
