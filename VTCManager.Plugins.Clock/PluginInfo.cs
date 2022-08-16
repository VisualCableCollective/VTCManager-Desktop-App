using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTCManager.Plugins.PluginBase;

namespace VTCManager.Plugins.Clock
{
    public class PluginInfo : IPluginInfo
    {
        public override string Name => "Clock";

        public override PluginType PluginType => PluginType.Widget;

        public override Type WidgetControlType => typeof(ClockWidget);
    }
}
