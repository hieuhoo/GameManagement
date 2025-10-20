using BlazorAppIdolJav.Share.Model.EditModel;
using Microsoft.AspNetCore.Components;

namespace BlazorAppIdolJav.Components.Layout
{
    public partial class MainLayout
    {
        UserEditModel EditModel { get; set; } = new UserEditModel();

        bool loginVisible;
        bool isLoggedIn = false;

        async Task HandleLoginAsync()
        {
            try
            {
                isLoggedIn = true;      
                loginVisible = false;
                StateHasChanged();
            }
            catch
            {

            }
        }

        void ShowLoginModal()
        {
            loginVisible = true;
        }

        void Logout()
        {

        }
    }
}
