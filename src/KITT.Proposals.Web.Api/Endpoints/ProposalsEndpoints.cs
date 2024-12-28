using KITT.Proposals.Web.Api.Endpoints.Services;
using KITT.Proposals.Web.Models;
using KITT.Web.Shared.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KITT.Proposals.Web.Api.Endpoints;

public static class ProposalsEndpoints
{
    public static IEndpointRouteBuilder MapProposalsEndpoints(this IEndpointRouteBuilder builder)
    {
        var proposalsGroup = builder
            .MapGroup("api/proposals")
            //.RequireAuthorization()
            .WithParameterValidation();

        proposalsGroup
            .MapGet("", GetAllProposals)
            .WithName(nameof(GetAllProposals))
            .WithOpenApi();

        proposalsGroup
            .MapGet("{id:guid}", GetProposalDetail)
            .WithName(nameof(GetProposalDetail))
            .WithOpenApi();

        proposalsGroup
            .MapPatch("{id:guid}", AcceptProposal)
            .WithName(nameof(AcceptProposal))
            .WithOpenApi();

        proposalsGroup
            .MapDelete("{id:guid}", RejectProposal)
            .WithName(nameof(RejectProposal))
            .WithOpenApi();

        proposalsGroup
            .MapDelete("{id:guid}/refuse", RefuseProposal)
            .WithName(nameof(RefuseProposal))
            .WithOpenApi();

        proposalsGroup
            .MapPost("{id:guid}/schedule", ScheduleProposal)
            .WithName(nameof(ScheduleProposal))
            .WithOpenApi();

        return builder;
    }

    private static async Task<Ok<ProposalListModel>> GetAllProposals(
        ProposalsEndpointsServices services,
        int s = 10,
        ProposalsQueryModel.SortDirection sort = ProposalsQueryModel.SortDirection.Descending,
        ProposalStatus? st = null,
        string? q = null)
    {
        var proposals = await services.GetAllProposalsAsync(
            size: s,
            sort,
            query: q,
            status: st);

        return TypedResults.Ok(proposals);
    }

    private static async Task<Results<Ok<ProposalDetailModel>, NotFound>> GetProposalDetail(
        ProposalsEndpointsServices services,
        Guid id)
    {
        var proposal = await services.GetProposalDetailAsync(id);
        if (proposal is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(proposal);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> AcceptProposal(
        ProposalsEndpointsServices services,
        Guid id)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.BadRequest();
        }

        try
        {
            await services.AcceptProposalAsync(id);
            return TypedResults.NoContent();
        }
        catch (ArgumentOutOfRangeException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> RejectProposal(
        ProposalsEndpointsServices services,
        Guid id)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.BadRequest();
        }

        try
        {
            await services.RejectProposalAsync(id);
            return TypedResults.NoContent();
        }
        catch (ArgumentOutOfRangeException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> RefuseProposal(
        ProposalsEndpointsServices services,
        Guid id)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.BadRequest();
        }

        try
        {
            await services.RefuseProposalAsync(id);
            return TypedResults.NoContent();
        }
        catch (ArgumentOutOfRangeException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok, NotFound, BadRequest, ValidationProblem>> ScheduleProposal(
        ProposalsEndpointsServices services,
        ClaimsPrincipal user,
        Guid id,
        [FromBody] ScheduleProposalModel model)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.BadRequest();
        }

        try
        {
            await services.ScheduleProposalAsync(id, model, user.GetUserId());
            return TypedResults.Ok();
        }
        catch (ArgumentOutOfRangeException)
        {
            return TypedResults.NotFound();
        }
    }
}
