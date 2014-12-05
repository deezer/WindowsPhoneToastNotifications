using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.UI.Core;
using Deezer.WindowsPhone.UI;
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
                manager.Enqueue(notification2updated);

                // Assert
                Assert.AreEqual(2, manager.QueuedNotifications.Count);
                Assert.IsInstanceOfType(manager.QueuedNotifications.Last(), typeof(SimpleToastNotification));
                SimpleToastNotification resultNotification = manager.QueuedNotifications.Last() as SimpleToastNotification;
                Assert.AreSame(notification2updated.Title, resultNotification.Title);
                return null;
            });
        }

        [TestMethod]
        public async Task ToastNotificationManagerTest_Enqueue_ExistingNotification_ShouldUpdateExistingNotification2()
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
                
                manager.Enqueue(new SimpleToastNotification(){Title = "notification3", Id = "notification3"});
                manager.Enqueue(new SimpleToastNotification(){Title = "notification4", Id = "notification4"});
                manager.Enqueue(new SimpleToastNotification(){Title = "notification5", Id = "notification5"});

                // Act
                manager.Enqueue(notification2updated);
                var resultNotification = manager.GetToastById("test") as SimpleToastNotification;

                // Assert
                Assert.AreEqual(5, manager.QueuedNotifications.Count);
                Assert.IsInstanceOfType(manager.QueuedNotifications.Last(), typeof(SimpleToastNotification));
                Assert.AreSame(notification2updated.Title, resultNotification.Title);
                return null;
            });
        }

        [TestMethod]
        public async Task ToastNotificationManagerTest_GetToastById_UnknownId_ShouldReturnNull()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();
                ToastNotificationManager manager = new ToastNotificationManager(rootGrid);
                ToastNotificationBase notification1 = new SimpleToastNotification();
                manager.Enqueue(notification1);

                // Act
                ToastNotificationBase toast = manager.GetToastById("newTag");

                // Assert
                Assert.IsNull(toast);
                return null;
            });
        }        
        
        [TestMethod]
        public async Task ToastNotificationManagerTest_GetToastById_KnowId_ShouldReturnNotifcation()
        {
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                // Arrange
                Grid rootGrid = new Grid();
                ToastNotificationManager manager = new ToastNotificationManager(rootGrid);
                ToastNotificationBase notification1 = new SimpleToastNotification();
                manager.Enqueue(notification1);
                manager.Enqueue(new SimpleToastNotification(){Id = "TestId", Title = "success"});
                
                // Act
                ToastNotificationBase toast = manager.GetToastById("TestId");

                // Assert
                Assert.IsNotNull(toast);
                Assert.IsInstanceOfType(toast, typeof(SimpleToastNotification));
                SimpleToastNotification notification = toast as SimpleToastNotification;
                Assert.AreSame("success", notification.Title);
                return null;
            });
        }
    }
}
