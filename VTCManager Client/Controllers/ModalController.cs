using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace VTCManager_Client.Controllers
{
    public static class ModalController
    {
        private static List<Page> ModalQueue = new List<Page>();
        private static Timer NextModalTimer = new Timer(1000);
        private static bool InitDone = false;
        public static void Init()
        {
            NextModalTimer.Elapsed += NextNextModalTimerTimer_Elapsed;
            NextModalTimer.Start();
            InitDone = true;
        }

        private static void NextNextModalTimerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ShowModal();
        }

        public static void AddModalToQueue(Page page) => ModalQueue.Add(page);

        private static void ShowModal()
        {
            Task.Run(() =>
            {
                if (!InitDone)
                    return;
                if (ModalQueue.Count == 0)
                    return;
                NextModalTimer.Stop();
                Page nextpage = ModalQueue.First();
                ControllerManager.MainWindow.ShowModal(nextpage);
            });
        }

        public static void CloseCurrentModal()
        {
            Task.Run(() =>
            {
                ControllerManager.MainWindow.HideModal();
                ModalQueue.RemoveAt(0);
                NextModalTimer.Start();
            });
        }
    }
}