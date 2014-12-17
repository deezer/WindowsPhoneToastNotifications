using System;

namespace Deezer.WindowsPhone.UI
{
    public class ToastCompletedEventArgs : EventArgs
    {
        public ToastCompletedEventArgs(DismissStatus dismissStatus)
        {
            DismissStatus = dismissStatus;
        }

        public DismissStatus DismissStatus { get; private set; }
    }
}
