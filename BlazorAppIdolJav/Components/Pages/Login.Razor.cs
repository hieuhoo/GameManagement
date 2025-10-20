using BlazorAppIdolJav.Share.Extension;
using BlazorAppIdolJav.Share.Model.EditModel;
using Microsoft.AspNetCore.Components;

namespace BlazorAppIdolJav.Components.Pages
{
    public partial class Login : ComponentBase
    {
        UserEditModel EditModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new UserEditModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task HandleLoginAsync()
        {
            try
            {

            }
            catch
            {

            }
        }
    }
}
