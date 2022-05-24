using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace VTCManager_Client.Controllers
{
    public static class NotificationController
    {
        public enum NotificationType
        {
            JobStarted,
            JobDelivered,
            JobCancelled,
        }
        public struct NotificationData
        {
            public NotificationData(NotificationType ntype, string strValue)
            {
                NType = ntype;
                Text = strValue;
            }

            public string Text { get; private set; }
            public NotificationType NType { get; private set; }
        }
        private static Windows.MainWindow mainWindow;
        private static List<NotificationData> NotificationQueue = new List<NotificationData>();
        private static System.Timers.Timer NextNotificationTimer = new System.Timers.Timer(1000);
        private static bool InitDone = false;
        public static void Init()
        {
            //get the main window to show the notifications
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(Windows.MainWindow))
                    {
                        mainWindow = (window as Windows.MainWindow);
                    }
                }
            });
            NextNotificationTimer.Elapsed += NextNotificationTimer_Elapsed;
            NextNotificationTimer.Start();
            InitDone = true;
        }

        private static void NextNotificationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ShowNotification();
        }

        public static void Notificate(NotificationType notificationType, string Text)
        {
            NotificationQueue.Add(new NotificationData(notificationType, Text));
        }

        private static void ShowNotification()
        {
            Task.Run(() =>
            {
                if (!InitDone)
                    return;
                if (NotificationQueue.Count == 0)
                    return;
                NextNotificationTimer.Stop();
                NotificationData notificationData = NotificationQueue.First();
                mainWindow.dashPage.ShowNotifcation(notificationData.Text);
                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    mainWindow.dashPage.CloseNotifcation();
                    NotificationQueue.RemoveAt(0);
                    NextNotificationTimer.Start();
                });
            });
        }
    }
}