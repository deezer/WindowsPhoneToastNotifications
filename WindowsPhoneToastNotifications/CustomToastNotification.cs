using System.Windows;
using System.Windows.Controls;

namespace Deezer.WindowsPhone.UI
{
    public class CustomToastNotification : ToastNotificationBase
    {
        public object Content { get; set; }

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            "ContentTemplate", typeof(DataTemplate), typeof(SimpleToastNotification), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ContentTemplate { get; set; }

        protected override ContentPresenter GetNotificationContent()
        {
            ContentPresenter contentControl = new ContentPresenter();
            contentControl.ContentTemplate = ContentTemplate;
            contentControl.OnApplyTemplate();
            contentControl.Content = Content;
            return contentControl;
        }
    }
}
