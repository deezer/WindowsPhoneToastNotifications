using System;
using System.Windows.Controls;

namespace WindowsPhoneToastNotifications
{
    public class ToastNotificationManager
    {
        public Grid RootGrid { get; protected set; }
        
        public ToastNotificationManager(Grid rootGrid)
        {
            if(rootGrid == null)
                throw new ArgumentNullException("rootGrid");

            RootGrid = rootGrid;
            RootGrid.Children.Add(new TextBlock());
        }
    }
}
