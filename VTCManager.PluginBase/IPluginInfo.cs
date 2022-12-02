using System;
using System.Timers;

namespace VTCManager.Plugins.PluginBase
{
    public abstract class IPluginInfo
    {
        public abstract string Name { get; }

        public abstract PluginType PluginType { get; }

        public virtual Type WidgetControlType { get; } = null;

        public virtual Type PageControlType { get; } = null;

        public virtual SidebarPluginInfo SidebarPluginInfo { get; } = null;

        public virtual Timer BackgroundWorker { get; } = null;
    }
}
