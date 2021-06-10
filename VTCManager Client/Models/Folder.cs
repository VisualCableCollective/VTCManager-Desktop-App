using System;

namespace VTCManager_Client.Models
{
    public static class Folder
    {
        public static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VTCManager\";
    }
}
