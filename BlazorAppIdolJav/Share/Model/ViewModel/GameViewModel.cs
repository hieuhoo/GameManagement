using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace GameManagement.Share.Model.ViewModel
{
    public class GameViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Stt { get; set; }
        public string Status { get; set; }
        public string SoldStatus { get; set; }
        public string ReleaseStatus { get; set; }
        public int Price { get; set; }
        public bool IsFeatured { get; set; }
        public int? OrderFeatured { get; set; }
        public string GameCompanyId { get; set; }
        public string GameCompanyName { get; set; }

        public string Unit { get; set; }
        public string DisplayPrice => $"{Price:N0} {Unit}";
             
    }
}
