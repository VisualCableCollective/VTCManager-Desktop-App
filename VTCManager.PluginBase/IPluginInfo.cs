using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VTCManager.Plugins.PluginBase
{
    public abstract class IPluginInfo
    {
        public abstract string Name { get; }

        public abstract PluginType PluginType { get; }

        public virtual Type WidgetControlType { get; } = null;
    }
}
