using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.UI.Core;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace WindowsPhoneToastNotifications.Test
{
    [TestClass]
    public class ToastNotificationManagerTest
    {
        #region ctor
        [TestMethod]
        public void ToastNotificationManagerTest_Ctor_WithtoutRoot_ShouldRaiseAnError()
        {
            // Arrange
            Grid rootGrid = null;
            
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new ToastNotificationManager(rootGrid));
        }

        [TestMethod]
        public async Task ToastNotificationManagerTest_Ctor_WithRoot_ShouldBeOk()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();

                // Act
                ToastNotificationManager actual = new ToastNotificationManager(rootGrid);

                // Assert
                Assert.IsNotNull(actual.RootGrid);
                Assert.AreEqual(rootGrid, actual.RootGrid);
                return null;
            });
        }
        #endregion

        [TestMethod]
        public async Task ToastNotificationManagerTest_Enqueue_ShouldAddANotification()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();
                ToastNotificationManager manager = new ToastNotificationManager(rootGrid);
                ToastNotificationBase notification = new SimpleToastNotification();
                
                // Act
                manager.Enqueue(notification);

                // Assert
                Assert.AreEqual(1, manager.QueuedNotifications.Count);
                return null;
            });
        }


        [TestMethod]
        public async Task ToastNotificationManagerTest_Enqueue_ShouldBeTheLastItem()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();
                ToastNotificationManager manager = new ToastNotificationManager(rootGrid);
                ToastNotificationBase notification1 = new SimpleToastNotification();
                ToastNotificationBase notification2 = new SimpleToastNotification();
                manager.Enqueue(notification1);

                // Act
                manager.Enqueue(notification2);

                // Assert
                Assert.AreEqual(2, manager.QueuedNotifications.Count);
                Assert.AreEqual(notification2, manager.QueuedNotifications.Last());
                return null;
            });
        }

        [TestMethod]
        public async Task ToastNotificationManagerTest_Enqueue_ExistingNotification_ShouldUpdateExistingNotification()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();
                ToastNotificationManager manager = new ToastNotificationManager(rootGrid);
                ToastNotificationBase notification1 = new SimpleToastNotification();
                ToastNotificationBase notification2 = new SimpleToastNotification();
                notification2.Id = "test";
                manager.Enqueue(notification1);
                manager.Enqueue(notification2);

                SimpleToastNotification notification2updated = new SimpleToastNotification();
                notification2updated.Id = "test";
                notification2updated.Title = "Updated content";
                
                // Act
                manager.Enqueue(notification2);

                // Assert
                Assert.AreEqual(2, manager.QueuedNotifications.Count);
                Assert.AreEqual(notification2updated, manager.QueuedNotifications.Last());
                return null;
            });
        }

    }
}
