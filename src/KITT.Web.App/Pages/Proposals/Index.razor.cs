using KITT.Web.App.Clients;
using KITT.Web.Models.Proposals;
using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Pages.Proposals;

public partial class Index
{
    private ProposalsQueryModel query = new();

    private ProposalListModel model = new();

    private bool isLoading = false;

    private int[] sizes = new[] { 10, 25, 50 };

    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    async Task Search()
    {
        await LoadProposalsAsync(query);
    }

    async void ClearSearch()
    {
        query = new();
        await LoadProposalsAsync(query);
        StateHasChanged();
    }

    private async Task LoadProposalsAsync(ProposalsQueryModel query)
    {
        try
        {
            isLoading = true;
            model = await Client.GetAllProposalsAsync(query);
        }
        finally
        {
            isLoading = false;
        }
    }
}
