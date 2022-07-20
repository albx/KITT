using KITT.Web.App.Clients;
using KITT.Web.Models.Proposals;
using Microsoft.AspNetCore.Components;

namespace KITT.Web.App.Pages.Proposals;

public partial class Index
{
    private ProposalsQueryModel query = new();

    private ProposalListModel model = new();

    private int[] sizes = new[] { 10, 25, 50 };

    [Inject]
    public IProposalsClient Client { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadProposalsAsync(query);
    }

    async Task Search()
    {
        await LoadProposalsAsync(query);
    }

    void ClearSearch() => query = new();

    private async Task LoadProposalsAsync(ProposalsQueryModel query)
        => model = await Client.GetAllProposalsAsync(query);
}
