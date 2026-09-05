using GameManagement.Share.ClassData;
using Microsoft.AspNetCore.Components;

namespace GameManagement.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [CascadingParameter] public UserData UserData { get; set; }
        [Parameter] public bool HaveLogged { get; set; }
        [Parameter] public bool IsAdmin { get; set; }

    }
}
