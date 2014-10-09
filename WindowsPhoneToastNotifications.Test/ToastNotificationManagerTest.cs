using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace WindowsPhoneToastNotifications.Test
{
    [TestClass]
    public class ToastNotificationManagerTest
    {
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

    }
}
