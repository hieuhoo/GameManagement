using System.ComponentModel.DataAnnotations;

namespace BlazorAppIdolJav.Share.Extension
{
    public class EnumExtension
    {
        public enum National
        {
            [Display(Name = "Nhật Bản")]
            Japan = 0,
            [Display(Name = "Mỹ")]
            America = 1,
            [Display(Name = "Đài Loan")]
            Taiwan,
            [Display(Name = "Việt Nam")]
            VietNam,
        }

        public enum CharacterGender
        {
            [Display(Name = "Nam")]
            Male = 0,
            [Display(Name = "Nữ")]
            Female = 1,
            [Display(Name = "Khác")]
            Other = 2,
        }

        public enum FilmDetailType
        {
            [Display(Name = "Luận loan")]
            Incestuous = 0,
            [Display(Name = "Hấp diêm")]
            Ravishing = 1,
            [Display(Name = "Mẹ vợ con rể")]
            MotherAndSonInLaw = 2,
            [Display(Name = "Học sinh")]
            Student = 3,
            [Display(Name = "Bố chồng nàng dâu")]
            FatherAndBride_In_Law = 4,
        }
    }
}
