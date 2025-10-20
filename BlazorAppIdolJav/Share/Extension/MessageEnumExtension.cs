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
        }

        public enum TypeAlert
        {
            [Display(Name = "Thông báo")]
            ThongBao = 0,
        }
    }
}
