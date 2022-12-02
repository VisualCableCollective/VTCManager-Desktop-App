using System;
using System.Threading.Tasks;
using System.Timers;
using VTCManager.Plugins.PluginBase;

namespace VTCManager.Plugins.ModSyncPro
{
    public class PluginInfo : IPluginInfo
    {
        public override string Name => "ModSyncPro";

        public override PluginType PluginType => PluginType.Ui;

        public override Type PageControlType => typeof(ModSyncProPage);

        public override SidebarPluginInfo SidebarPluginInfo => new SidebarPluginInfo("Mod Sync", null);

        public Task StartupTask = new Task(Init);

        private static void Init()
        {

        }
    }
}
