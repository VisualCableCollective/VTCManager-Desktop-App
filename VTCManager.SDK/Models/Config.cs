using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager.SDK.Models
{
    /// <summary>
    /// Configuration for <see cref="VTCManagerClient"/>
    /// </summary>
    public class Config : PropertiesChangeable
    {
        #region Features
        /// <summary>
        /// Set this to <see langword="true"/> if the Discord RPC features should be enabled.
        /// </summary>
        public bool EnableDiscordRPCFeature
        {
            get => _enableDiscordRPCFeature;
            set => SetField(ref _enableDiscordRPCFeature, value);
        }
        private bool _enableDiscordRPCFeature = true;
        #endregion
    }
}
