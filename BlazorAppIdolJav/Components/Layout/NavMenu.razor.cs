using Microsoft.AspNetCore.Components;

namespace BlazorAppIdolJav.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Parameter] public bool HaveLogged {get;set;}
    }
}
