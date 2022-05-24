namespace VTCManager_Client.Models
{
    public class Config
    {
        public bool ETS_Plugin_Installed = false;
        public bool ATS_Plugin_Installed = false;
        public bool Debug = false;
        public string last_version_used = null;
        public string last_deploy_version_used = "";
        public Models.User User = new Models.User();
        public byte[] ADataEntropy = null; 
        public int ADataBytesWritten = 0;
        public string ADataSHA256Hash = null;
        public bool DiscordRPC_Enabled = true;
        public string GameLanguageCode = null;
        public bool User_Disabled_Auto_Start = false;
    }
}