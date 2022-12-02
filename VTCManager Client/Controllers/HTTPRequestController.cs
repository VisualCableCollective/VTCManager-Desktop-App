using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using VTCManager.Logging;

namespace VTCManager_Client.Controllers
{
    public static class HTTPRequestController
    {
        private static string LogPrefix = "[HTTPRequestController] ";
        public enum Service
        {
            VTCManager,
        }
        public static Dictionary<string, string> Request(string url, Service service, string Method = "GET", Dictionary<string, string> postdata = null)
        {
            LogController.Write(LogPrefix + "Executing a new request...", LogController.LogType.Debug);
            Dictionary<string, string> return_str = new Dictionary<string, string>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = Method;
            request.UserAgent = AppInfo.AppName + " " + AppInfo.Version;
            request.Accept = "application/json";
            switch (service)
            {
                case Service.VTCManager:
                    if (!String.IsNullOrEmpty(AuthDataController.GetAPIToken()))
                    {
                        request.Headers.Add("Authorization", "Bearer " + AuthDataController.GetAPIToken());
                    }
                    break;
                default:
                    return return_str;
            }

            if (postdata != null)
            {
                string s = "";
                foreach (string str2 in postdata.Keys)
                {
                    string[] textArray1 = new string[] { s, HttpUtility.UrlEncode(str2), "=", HttpUtility.UrlEncode(postdata[str2]), "&" };
                    s = string.Concat(textArray1);
                }
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                try
                {
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                catch (Exception ex)
                {
                    LogController.Write(LogPrefix + "Failed to get the request Stream. URL: " + url + " | Error: " + ex.Message, LogController.LogType.Error);
                    return_str.Add("status_code", "REQUEST_FAILED");
                    return_str.Add("response", "REQUEST_FAILED");
                    return return_str;
                }
            }
            HttpWebResponse response = null;
            string statusCode = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                statusCode = ((int)response.StatusCode).ToString();
            }
            catch (WebException ex)
            {
                LogController.Write(LogPrefix + "Request Status: " + ex.Status.ToString(), LogController.LogType.Debug);
                if (ex.Status.ToString() == "OK")
                {
                    response = ex.Response as HttpWebResponse;
                    statusCode = ((int)response.StatusCode).ToString();
                }
            }
            if (response != null)
            {
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                string response_data = sr.ReadToEnd();
                sr.Close();
                resStream.Close();
                LogController.Write(LogPrefix + "Request StatusCode: " + statusCode, LogController.LogType.Debug);
                LogController.Write(LogPrefix + "Request Response: " + response_data, LogController.LogType.Debug);
                return_str.Add("status_code", statusCode);
                return_str.Add("response", response_data);
            }
            else
            {
                LogController.Write(LogPrefix + "Request failed. URL: " + url, LogController.LogType.Error);
                return_str.Add("status_code", "REQUEST_FAILED");
                return_str.Add("response", "REQUEST_FAILED");
            }
            return return_str;
        }
    }
}