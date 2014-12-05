using System;

namespace Deezer.WindowsPhone.UI
{
    public class ToastCompletedEventArgs : EventArgs
    {
        public ToastCompletedEventArgs(DismissStatus hasBeenDismissed)
        {
            HasBeenDismissed = hasBeenDismissed;
        }

        public DismissStatus HasBeenDismissed { get; set; }
    }
}
