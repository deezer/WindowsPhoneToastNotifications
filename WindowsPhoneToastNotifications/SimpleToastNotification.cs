using System;
using System.Windows;
using System.Windows.Controls;

namespace Deezer.WindowsPhone.UI
{
    public class SimpleToastNotification : ToastNotificationBase
    {
        private const string DefaultContentTemplateResourceKey = "SimpleToastNotificationContentTemplate";
        public string Title { get; set; }
        public string Content { get; set; }

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            "ContentTemplate", typeof (DataTemplate), typeof (SimpleToastNotification), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ContentTemplate { get; set; }

        protected override ContentPresenter GetNotificationContent()
        {
            ContentPresenter contentControl = new ContentPresenter();
            
            if (ContentTemplate == null)
            {
                if (Application.Current.Resources.Contains(DefaultContentTemplateResourceKey))
                {
                    ContentTemplate = Application.Current.Resources[DefaultContentTemplateResourceKey] as DataTemplate;
                }
            }

            contentControl.ContentTemplate = ContentTemplate;
            contentControl.OnApplyTemplate();
            contentControl.Content = new { Title, Content };
            return contentControl;
        }
    }
}
