using AntDesign;
using BlazorAppIdolJav.Share.Extension;
using static BlazorAppIdolJav.Share.Extension.MessageEnumExtension;

namespace BlazorAppIdolJav.SpecialComponent.ExtensionClass
{
    public static class AntNoticeExtension
    {
        private static void NoticeWithIcon(this NotificationService notificationService, NotificationType type, string message)
        {
            _ = notificationService.Open(new NotificationConfig()
            {
                Message = TypeAlert.ThongBao.ToString(),
                Description = message,
                NotificationType = type,
                Duration = 2,
            });
        }

        public static void NotiSuccess(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.Success, message);
        }

        public static void NotiWarning(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.Warning, message);
        }

        public static void NotiError(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.Error, message);
        }

        public static void NotiInfo(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.Info, message);
        }

        public static void NotiNone(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.None, message);
        }

        public static void NotiDeleteError(this NotificationService notificationService, string message)
        {
            notificationService.NoticeWithIcon(NotificationType.Error, message ?? OperationEnum.DeleteFailed.ToString());
        }

    }
}
