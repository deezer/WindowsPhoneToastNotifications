using System;

namespace WindowsPhoneToastNotifications
{
    public class SimpleToastNotification : ToastNotificationBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Uri IconUri { get; set; }
    }
}
