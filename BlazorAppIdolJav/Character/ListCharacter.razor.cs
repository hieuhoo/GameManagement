using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using BlazorAppIdolJav.Data;
using BlazorAppIdolJav.Share.Model.ClassModel;

namespace BlazorAppIdolJav.Character
{
    public partial class ListCharacter : ComponentBase
    {
        [Inject] ApplicationDbContext Db { get; set; }

        protected List<Actress> Character = new List<Actress>();

        protected override async Task OnInitializedAsync()
        {
            Character = await Db.Actress.ToListAsync();
        }
    }
}
