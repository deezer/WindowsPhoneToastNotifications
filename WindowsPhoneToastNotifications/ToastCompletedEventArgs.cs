using System;

namespace Deezer.WindowsPhone.UI
{
    public class ToastCompletedEventArgs : EventArgs
    {
        public ToastCompletedEventArgs(bool hasBeenDismissed)
        {
            HasBeenDismissed = hasBeenDismissed;
        }

        public bool HasBeenDismissed { get; set; }
    }
}
