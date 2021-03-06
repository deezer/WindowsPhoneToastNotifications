﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Deezer.WindowsPhone.UI
{
    public class ToastNotificationManager
    {
        private readonly List<ToastNotificationBase> _notificationsQueue;
        private readonly object _notificationQueueLock = new object();

        public IReadOnlyList<ToastNotificationBase> QueuedNotifications
        {
            get { return _notificationsQueue; }
        }

        protected Grid RootGrid { get; set; }

        public ToastNotificationBase CurrentNotification { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastNotificationManager"/> class.
        /// </summary>
        /// <param name="rootGrid">The parent grid to use for the notification.</param>
        /// <exception cref="System.ArgumentNullException">rootGrid</exception>
        public ToastNotificationManager(Grid rootGrid)
        {
            if (rootGrid == null)
                throw new ArgumentNullException("rootGrid");

            RootGrid = rootGrid;
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
            if (CurrentNotification != null && CurrentNotification.Id == toastNotification.Id)
            {
                SwipeCurrentNotification(toastNotification);
                return;
            }


            // else-if notification is on queue, replace existing instance
            if (_notificationsQueue.Any(notification => notification.Id == toastNotification.Id && notification.Id != null))
            {
                InternalReplaceQueueItem(toastNotification);
            }
            else
            {
                // else add to queue
                InternalAddToQueue(toastNotification);
            }

            if (CurrentNotification == null)
            {
                ShowNextNotification();
            }
        }

        private void SwipeCurrentNotification(ToastNotificationBase toastNotification)
        {
            lock (_notificationQueueLock)
            {
                CurrentNotification.AbortNotification();
                CurrentNotification.CompleteToast(DismissStatus.Dismissed, notifyManager: false);
                _notificationsQueue.Remove(CurrentNotification);
                CurrentNotification = toastNotification;
                CurrentNotification.Show(this);
            }
        }

        /// <summary>
        /// Removes a notification from the queue.
        /// </summary>
        /// <param name="toastNotification">The <see cref="ToastNotificationBase"/> to remove.</param>
        public void Dequeue(ToastNotificationBase toastNotification)
        {
            if (CurrentNotification == toastNotification)
            {
                toastNotification.CompleteToast(DismissStatus.InternalDismissed);
            }

            lock (_notificationQueueLock)
            {
                _notificationsQueue.Remove(toastNotification);
            }
        }

        /// <summary>
        /// Gets the toast by identifier.
        /// </summary>
        /// <param name="toastId">The toast identifier.</param>
        /// <returns>ToastNotificationBase.</returns>
        public ToastNotificationBase GetToastById(string toastId)
        {
            return _notificationsQueue.FirstOrDefault(notification => notification.Id == toastId);
        }

        /// <summary>
        /// Removes all pending notifications from the queue.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        internal void AddNotificiationToVisualTree(ToastNotificationBase toastNotificationBase)
        {
             if(toastNotificationBase == null)
                 throw new ArgumentNullException("toastNotificationBase");

            if(toastNotificationBase.ToastControlMainBorder == null)
                throw new ArgumentNullException("toastNotificationBase", "ToastNotificationBase object must contain a ToastControlMainBorder");

            RootGrid.Children.Add(toastNotificationBase.ToastControlMainBorder);
        }

        internal void RemoveNotificationFromVisualTree(ToastNotificationBase toastNotificationBase)
        {
            if (toastNotificationBase == null)
                throw new ArgumentNullException("toastNotificationBase");

            if (toastNotificationBase.ToastControlMainBorder == null)
                throw new ArgumentNullException("toastNotificationBase", "ToastNotificationBase object must contain a ToastControlMainBorder");

            RootGrid.Children.Remove(toastNotificationBase.ToastControlMainBorder);
        }

        private void ShowNextNotification()
        {
            lock (_notificationQueueLock)
            {
                CurrentNotification = _notificationsQueue.FirstOrDefault();

                if (CurrentNotification == null)
                    return;

                CurrentNotification.Show(this);
            }
        }

        private void InternalAddToQueue(ToastNotificationBase toastNotification)
        {
            lock (_notificationQueueLock)
            {
                _notificationsQueue.Add(toastNotification);
            }
        }

        private void InternalReplaceQueueItem(ToastNotificationBase toastNotification)
        {
            lock (_notificationQueueLock)
            {
                ToastNotificationBase actualNotification = _notificationsQueue.FirstOrDefault(notification => notification.Id == toastNotification.Id && notification.Id != null);
                if (actualNotification == null) throw new ArgumentNullException("actualNotification");

                actualNotification.CompleteToast(dismissStatus: DismissStatus.InternalDismissed);

                int actualNotificationIndex = _notificationsQueue.IndexOf(actualNotification);
                _notificationsQueue.Remove(actualNotification);
                _notificationsQueue.Insert(actualNotificationIndex, toastNotification);
            }
        }

        public void CompleteToast(ToastNotificationBase toastNotificationBase)
        {
            CurrentNotification = null;
            Dequeue(toastNotificationBase);
            ShowNextNotification();
        }
    }
}
