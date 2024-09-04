﻿using KITT.Web.App.Clients;
using KITT.Web.App.Components;
using KITT.Web.App.UI;
using KITT.Web.Models.Proposals;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace KITT.Web.App.Pages.Proposals;

public partial class Schedule
{
    [Parameter]
    public Guid Id { get; set; }

    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    [Inject]
    public IMessageService MessageService { get; set; } = default!;

    private ScheduleForm.ViewModel model = new();

    private bool isLoading = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        isLoading = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            await base.OnParametersSetAsync();

            var proposalDetail = await Client.GetProposalDetailAsync(Id);
            if (proposalDetail is not null)
            {
                model = ConvertToViewModel(proposalDetail);
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private ScheduleForm.ViewModel ConvertToViewModel(ProposalDetailModel proposalDetail)
    {
        return new ScheduleForm.ViewModel
        {
            Title = proposalDetail.Title,
            StreamingAbstract = proposalDetail.Description
        };
    }

    async Task ScheduleProposalAsync(ScheduleForm.ViewModel model)
    {
        try
        {
            var scheduleProposalModel = ConvertToApiModel(model);

            await Client.ScheduleProposalAsync(Id, scheduleProposalModel);
            Navigation.NavigateTo("proposals");
        }
        catch (Exception ex)
        {
            await MessageService.ShowMessageBarAsync(
                ex.Message,
                MessageIntent.Error,
                SectionNames.MessagesTopSectionName);
        }
    }

    private ScheduleProposalModel ConvertToApiModel(ScheduleForm.ViewModel model)
    {
        if (model.ScheduleDate is null)
        {
            throw new ArgumentNullException(nameof(model.ScheduleDate));
        }

        if (model.StartingTime is null)
        {
            throw new ArgumentNullException(nameof(model.StartingTime));
        }

        if (model.EndingTime is null)
        {
            throw new ArgumentNullException(nameof(model.EndingTime));
        }

        return new ScheduleProposalModel
        {
            Title = model.Title,
            ScheduleDate = model.ScheduleDate.Value,
            EndingTime = model.EndingTime.Value.TimeOfDay,
            HostingChannelUrl = $"https://www.twitch.tv/{model.HostingChannelUrl}",
            Slug = model.Slug,
            StartingTime = model.StartingTime.Value.TimeOfDay,
            StreamingAbstract = model.StreamingAbstract,
            Seo = model.Seo
        };
    }
}
