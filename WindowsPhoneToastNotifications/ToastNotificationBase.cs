using System.Diagnostics;

namespace Deezer.WindowsPhone.UI
{
    [DebuggerDisplay("ToastNotificationBase({Id})")]
    public abstract class ToastNotificationBase
    {
        public string Id { get; set; }
    }
}
