using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Extension
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
            [Display(Name = "Trung Quốc")]
            China,
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


        public enum UserRole
        {
            [Display(Name = "ADMIN")]
            Admin = 0,
            [Display(Name = "Bình thường")]
            Normal = 1,
        }

        public enum GameStatus
        {
            [Display(Name = "Chưa ra mắt")]
            UnReleased = 0,
            [Display(Name = "Đã ra mắt")]
            Released = 1,
        }

        public enum SoldStatus
        {
            [Display(Name = "Hết hàng")]
            OutOfStock = 0,
            [Display(Name = "Còn hàng")]
            InStock = 1,
        }

        public enum PlatformSystem
        {
            [Display(Name = "Window")]
            Window = 0,
            [Display(Name = "MacOS")]
            MacOS = 1,
        }

        public enum UnitMoneyEnum
        {
            [Display(Name = "VNĐ")]
            VNĐ = 0,
            [Display(Name = "USD")]
            USD = 1,
        }

        public enum StatusEnum
        {
            [Display(Name = "Hiệu lực")]
            Active = 0,
            [Display(Name = "Không hiệu lực")]
            Disable = 1,
        }

        public enum GeneralGameSetup
        {
            [Display(Name = "Hãng game")]
            GameCompany = 0,
            [Display(Name = "Thể loại game")]
            GameType = 1,
        }
    }
}
