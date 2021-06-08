using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager_Client.Controllers
{
    public static class AuthDataController
    {
        private static string AuthDataFilePath;
        private static string LogPrefix = "[AuthDataController] ";
        private static string API_Token = null;
        private static bool Adata_FileOpen = false;

        public static Models.ControllerStatus Init()
        {
            AuthDataFilePath = StorageController.AppDataFolder + "adata.dat";
            if (!File.Exists(AuthDataFilePath))
            {
                //so we don't open the not existing file by mistake
                StorageController.Config.ADataEntropy = null;
                StorageController.Config.ADataBytesWritten = 0;
            }

            ConvertOldStorageToNew();

            return Models.ControllerStatus.OK;
        }

        private static void SaveData(String data)
        {
            byte[] toEncrypt = UnicodeEncoding.ASCII.GetBytes(data);
            StorageController.Config.ADataEntropy = CreateRandomEntropy();

            FileStream filestream;
            try
            {
                filestream = new FileStream(AuthDataFilePath, FileMode.OpenOrCreate);
            }catch(Exception ex)
            {
                LogController.Write(LogPrefix + "Couldn't create adata file: " + ex.Message, LogController.LogType.Error);
                return;
            }

            StorageController.Config.ADataBytesWritten = EncryptDataToStream(toEncrypt, StorageController.Config.ADataEntropy, DataProtectionScope.CurrentUser, filestream);
            SHA256 sha = SHA256.Create();
            StorageController.Config.ADataSHA256Hash = System.Text.Encoding.Default.GetString(sha.ComputeHash(filestream));
            filestream.Close();
        }

        private static string ReadDataFromFile()
        {
            if (Adata_FileOpen)
            {
                while (Adata_FileOpen)
                {

                }
                return API_Token;
            }
            Adata_FileOpen = true;
            // Open the file.
            FileStream fStream;
            try
            {
                fStream = new FileStream(AuthDataFilePath, FileMode.Open);
            }catch(Exception ex)
            {
                LogController.Write(LogPrefix + "Couldn't open Adata file: " + ex.Message, LogController.LogType.Error);
                Adata_FileOpen = false;
                return "";
            }
            // Read from the stream and decrypt the data.
            byte[] decryptData = DecryptDataFromStream(StorageController.Config.ADataEntropy, DataProtectionScope.CurrentUser, fStream, StorageController.Config.ADataBytesWritten);
            fStream.Close();

            API_Token = UnicodeEncoding.ASCII.GetString(decryptData);
            Adata_FileOpen = false;
            return API_Token;
        }

        public static string GetAPIToken()
        {
            if (API_Token != null)
                return API_Token;

            if (StorageController.Config.ADataEntropy == null || StorageController.Config.ADataBytesWritten == 0 || StorageController.Config.ADataSHA256Hash == null)
                return "";

            try
            {
                return ReadDataFromFile();
            }catch(Exception ex)
            {
                LogController.Write(LogPrefix + "An error occured while reading the auth data from the file: " + ex.Message, LogController.LogType.Error);
                API_Token = "";
                return "";
            }
        }

        public static void SetAPIToken(String token)
        {
            API_Token = token;
            SaveData(token);
        }

        private static byte[] DecryptDataFromStream(byte[] Entropy, DataProtectionScope Scope, Stream S, int Length)
        {
            if (S == null)
                throw new ArgumentNullException("S");
            if (Length <= 0)
                throw new ArgumentException("Length");
            if (Entropy == null)
                throw new ArgumentNullException("Entropy");
            if (Entropy.Length <= 0)
                throw new ArgumentException("Entropy");

            byte[] inBuffer = new byte[Length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.
            if (S.CanRead)
            {
                S.Read(inBuffer, 0, Length);

                outBuffer = ProtectedData.Unprotect(inBuffer, Entropy, Scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the length that was written to the stream.
            return outBuffer;
        }

        private static byte[] CreateRandomEntropy()
        {
            byte[] entropy = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(entropy);
            return entropy;
        }

        private static int EncryptDataToStream(byte[] Buffer, byte[] Entropy, DataProtectionScope Scope, Stream S)
        {
            if (Buffer == null)
                throw new ArgumentNullException("Buffer");
            if (Buffer.Length <= 0)
                throw new ArgumentException("Buffer");
            if (Entropy == null)
                throw new ArgumentNullException("Entropy");
            if (Entropy.Length <= 0)
                throw new ArgumentException("Entropy");
            if (S == null)
                throw new ArgumentNullException("S");

            int length = 0;

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] encryptedData = ProtectedData.Protect(Buffer, Entropy, Scope);

            // Write the encrypted data to a stream.
            if (S.CanWrite && encryptedData != null)
            {
                S.Write(encryptedData, 0, encryptedData.Length);

                length = encryptedData.Length;
            }

            // Return the length that was written to the stream.
            return length;
        }

        private static void ConvertOldStorageToNew()
        {
            if (!String.IsNullOrWhiteSpace(StorageController.Config.User.API_Token))
            {
                AuthDataController.SetAPIToken(StorageController.Config.User.API_Token);
                StorageController.Config.User.API_Token = null;
                LogController.Write(LogPrefix + "Moved api token to the new location! ");
            }
        }
    }
}
