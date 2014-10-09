using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace WindowsPhoneToastNotifications
{
    public class ToastNotificationManager
    {
        private List<ToastNotificationBase> _notificationsQueue;
        private object _notificationQueueLock = new object();

        public IReadOnlyList<ToastNotificationBase> QueuedNotifications
        {
            get { return _notificationsQueue; }
        }

        public Grid RootGrid { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastNotificationManager"/> class.
        /// </summary>
        /// <param name="rootGrid">The parent grid to use for the notification.</param>
        /// <exception cref="System.ArgumentNullException">rootGrid</exception>
        public ToastNotificationManager(Grid rootGrid)
        {
            if(rootGrid == null)
                throw new ArgumentNullException("rootGrid");

            RootGrid = rootGrid;
            RootGrid.Children.Add(new TextBlock());
            _notificationsQueue = new List<ToastNotificationBase>();
        }

        /// <summary>
        /// Enqueues a notification. If a notification with the same Id exists, 
        /// replace the existing notification.
        /// </summary>
        /// <param name="toastNotification">The <see cref="ToastNotificationBase"/> to enqueue or update.</param>
        public void Enqueue(ToastNotificationBase toastNotification)
        {
            // If the toastNotification is on screen, replace datacontext
            // TODO: Implement live notification swapping

            // else-if notification is on queue, replace existing instance


            // else add to queue
            InternalAddToQueue(toastNotification);
        }

        /// <summary>
        /// Removes a notification from the queue.
        /// </summary>
        /// <param name="toastNotification">The <see cref="ToastNotificationBase"/> to remove.</param>
        public void Dequeue(ToastNotificationBase toastNotification)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }


        /// <summary>
        /// Removes all pending notifications from the queue.
        /// </summary>
        public void Clear()
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        private void InternalAddToQueue(ToastNotificationBase toastNotification)
        {
            lock (_notificationQueueLock)
            {
                _notificationsQueue.Add(toastNotification);
            }
        }
    }
}
