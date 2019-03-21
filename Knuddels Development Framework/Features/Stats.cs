using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Management;
using System.Reflection;

namespace KDF.Features
{
    internal static class Stats
    {
        const string baseAddress = "http://k-scene.net";
        const string hwidAddress = "/coding/service/stats/stats.php?hwid=";
        static string appName = Assembly.GetEntryAssembly().FullName;

        internal static void SendStart()
        {
            try
            {
               
            }
            catch { }
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
