using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VTCManager.Plugins.PluginBase
{
    public sealed class SidebarPluginInfo
    {
        public SidebarPluginInfo(string text, ImageSource icon)
        {
            Text = text;
            Icon = icon;
        }

        public string Text { get; }

        public ImageSource Icon { get; }
    }
}
