using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.ViewModel
{
    public class GameViewModel
    {
        public virtual string Id { get; set; }
        public virtual string CompanyId { get; set; }
        [Display(Name = "STT")]
        public int Stt { get; set; }
        public int ChestSize { get; set; }
        public int WaistSize { get; set; }
        public int ButtSize { get; set; }
        public string NickName { get; set; }
        public string  Name { get; set; }
        public string Country { get; set; }

        public int Age { get; set; }
        public string Gender { get; set; }


    }
}
