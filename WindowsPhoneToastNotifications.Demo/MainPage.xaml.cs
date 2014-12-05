using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Deezer.WindowsPhone.UI;
using Microsoft.Phone.Controls;

namespace WindowsPhoneToastNotifications.Demo
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ToastNotificationManager _notificationManager;

        public MainPage()
        {
            InitializeComponent();
            _notificationManager = new ToastNotificationManager(LayoutRoot);
        }

        private void OnSimpleTextToastButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Content = "Bonjour " + DateTime.UtcNow.ToString("T");
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

        private void OnToastResultButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Title = "Do you want to tap me ?";
            simpleToastNotification.Completed += OnSimpleToastResultTapped;
            _notificationManager.Enqueue(simpleToastNotification);
        }

        private void OnSimpleToastResultTapped(object sender, ToastCompletedEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = sender as SimpleToastNotification;
            if (simpleToastNotification == null)
                return;

            simpleToastNotification.Completed -= OnSimpleToastResultTapped;
            MessageBox.Show("Toast has been dismissed: " + e.HasBeenDismissed);
        }

        private async void OnSimpleAsyncTextToastButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Title = "Do you want to tap me async ?";
            DismissStatus result = await simpleToastNotification.EnqueueAndShow(_notificationManager);
            MessageBox.Show("Toast has been dismissed: " + result);
        }

        private void OnCustomToastButtonTapped(object sender, GestureEventArgs e)
        {
            CustomToastNotification customToastNotification = new CustomToastNotification();
            customToastNotification.BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x2B, 0xCA, 0xB2));
            customToastNotification.ContentTemplate = App.Current.Resources["AlbumToastNotificationContentTemplate"] as DataTemplate;
            customToastNotification.Content = new { Title = "Unconditionally", ArtistName = "Katy Perry", PictureUri = new Uri("http://api.deezer.com/artist/144227/image") };
            _notificationManager.Enqueue(customToastNotification);
        }

        private async void OnSwipeNotificationStep1ButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Title = "PRISM has been added to favorites";
            simpleToastNotification.Id = "album.favoritestatus.27493";
            DismissStatus result = await simpleToastNotification.EnqueueAndShow(_notificationManager);
        }

        private async void OnSwipeNotificationStep2ButtonTapped(object sender, GestureEventArgs e)
        {
            SimpleToastNotification simpleToastNotification = new SimpleToastNotification();
            simpleToastNotification.Title = "PRISM has been removed to favorites";
            simpleToastNotification.Id = "album.favoritestatus.27493";
            DismissStatus result = await simpleToastNotification.EnqueueAndShow(_notificationManager);   
        }
    }
}