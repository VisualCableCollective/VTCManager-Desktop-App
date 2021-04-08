using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTCManager_Client.Models.API.VTCManager.StatusResponseObjects;

namespace VTCManager_Client.Models.API.VTCManager
{
    public class StatusResponse
    {
        public WebApp WebApp { get; set; }
        public DesktopClient DesktopClient { get; set; }
    }
}

namespace VTCManager_Client.Models.API.VTCManager.StatusResponseObjects
{
    public class WebApp
    {
        public bool operational { get; set; }
    }

    public class DesktopClient
    {
        public bool operational { get; set; }
    }
}
