using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Deezer.WindowsPhone.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhoneToastNotifications.Demo.Resources;

namespace WindowsPhoneToastNotifications.Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ToastNotificationManager _notificationManager;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            _notificationManager = new ToastNotificationManager(LayoutRoot);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void OnSimpleTextToastButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Content = "Bonjour " + DateTime.UtcNow.ToString("T");
            //simpleToastNotification.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x34, 0x64, 0x91));
            _notificationManager.Enqueue(simpleToastNotification);
        }

        private void OnSimpleTitleToastButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Title = "Toast";
            simpleToastNotification.Content = "Bonjour " + DateTime.UtcNow.ToString("T");
            simpleToastNotification.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x34, 0x64, 0x91));
            _notificationManager.Enqueue(simpleToastNotification);
        }
    }
}