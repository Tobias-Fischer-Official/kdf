using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Management;
using System.Reflection;

namespace KDF.Tools
{
    internal static class Stats
    {
        const string baseAddress = "http://k-scene.net/";
        const string hwidAddress = "/coding/service/stats/stats.php?hwid=";
        static string version = Application.ProductVersion;
        static string appName = Assembly.GetEntryAssembly().FullName;
        static string ratingString = string.Empty;
        static string feedbackString = string.Empty;

        internal static void SendFeedback(string feedback, string mail, string name, string topic)
        {
            feedbackString = baseAddress + hwidAddress + GetHwid() + "&request=feedback&type=" + topic + "&mail=" + mail + "&name=" + name + "&feedback=" + feedback + "&version=" + version + "&app=" + Tools.File2MD5.GetMD5(System.Windows.Forms.Application.ExecutablePath);
            MessageBox.Show("Feedback wird gesendet", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Thread thr = new Thread(SendFeedback);
            thr.Start();
        }

        internal static void SendFeedback()
        {
            try
            {
                new WebClient().DownloadStringAsync(new Uri(feedbackString));
                MessageBox.Show("Feedback wurde gesendet", "Danke für Ihre Meinung", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Feedback konnte nicht gesendet werden", "Fehler bei der Verbindung", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static void SendStart()
        {
            try
            {
                new WebClient().DownloadStringAsync(new Uri(baseAddress + hwidAddress + GetHwid() + "&request=start&version=" + version.ToString().Replace(".", string.Empty) +"&app=" + Tools.File2MD5.GetMD5(System.Windows.Forms.Application.ExecutablePath)));
            }
            catch { }
        }

        internal static void SendRating(string ratingtype, int rating)
        {
            ratingString = baseAddress + hwidAddress + GetHwid() + "&request=rating&type=" + ratingtype + "&rating=" + rating.ToString() + "&version=" + version.ToString().Replace(".", string.Empty) + "&app=" + Tools.File2MD5.GetMD5(System.Windows.Forms.Application.ExecutablePath);
            Thread thr = new Thread(SendRating);
            thr.Start();            
        }

        static void SendRating()
        {
            try
            {
                new WebClient().DownloadString(new Uri(ratingString));
            }
            catch
            {
                string t = ratingString.Substring(ratingString.IndexOf("type=" + 5));
                t = t.Substring(0, t.IndexOf("&"));
                MessageBox.Show("Bewertung für " + t + " konnte nicht gewertet werden, Verbindungsfehler.");
            }
        }

        internal static string GetHwid()
        {
            string drive = string.Empty;

            foreach (System.IO.DriveInfo compDrive in System.IO.DriveInfo.GetDrives())
                if (compDrive.IsReady)
                {
                    drive = compDrive.RootDirectory.ToString();
                    break;
                    
                }

            drive = drive.EndsWith(":\\") ? drive.Substring(0, drive.Length - 2) : drive;
            
            string volumeSerial = string.Empty;
            System.Management.ManagementObject disk = new System.Management.ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            string a = Environment.OSVersion.Version.ToString();
            string b = volumeSerial;
            string c = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();

            return a + b + c;
        }
    }
}
