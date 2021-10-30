using KITT.Web.App.Clients;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

namespace KITT.Web.App.Pages.Streamings
{
    public partial class Index
    {
        [Inject]
        public IStreamingsClient Client { get; set; }

        private StreamingsListModel model = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                model = await Client.GetAllStreamingsAsync();
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
