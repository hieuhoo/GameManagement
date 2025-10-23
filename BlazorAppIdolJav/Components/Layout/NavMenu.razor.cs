using GameManagement.Share.ClassData;
using Microsoft.AspNetCore.Components;

namespace GameManagement.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Parameter] public bool HaveLogged { get; set; }
        [CascadingParameter] public UserData UserData { get; set; }
    }
}
