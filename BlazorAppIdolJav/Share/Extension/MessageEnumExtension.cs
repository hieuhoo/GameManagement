using System.ComponentModel.DataAnnotations;

namespace BlazorAppIdolJav.Share.Extension
{
    public class MessageEnumExtension
    {
        public enum OperationEnum
        {
            [Display(Name = "Bạn có chắc chắn muốn xóa không?")]
            AreYouSureToDelete = 0,
            [Display(Name = "Xóa dữ liệu thất bại")]
            DeleteFailed = 1,
            [Display(Name = "Xóa dữ liệu thành công")]
            DeleteSucessfully = 2,
            [Display(Name = "Cập nhật dữ liệu thất bại")]
            UpdateFailed = 3,
            [Display(Name = "Cập nhật dữ liệu thành công")]
            UpdateSuccessfully = 4,
            [Display(Name = "Thêm dữ liệu thành công")]
            AddSuccessfully = 5,
            [Display(Name = "Thêm dữ liệu thất bại")]
            AddFailed = 6,
        }

        public enum TypeAlert
        {
            [Display(Name = "Thông báo")]
            ThongBao = 0,
            [Display(Name = "Dữ liệu bắt buộc nhập")]
            Required = 1,
            [Display(Name = "Dữ liệu còn thiếu hoặc không hợp lệ")]
            InvalidData = 2,
        }

        public enum AccountRegisterEnum
        {
            [Display(Name = "Tạo tài khoản thành công")]
            Success = 0,
            [Display(Name = "Tạo tài khoản thất bại")]
            Failed = 1,
            [Display(Name = "Tên đăng nhập hoặc Email đã tồn tại")]
            ExistEmailOrUserName = 2,
        }
    }
}
