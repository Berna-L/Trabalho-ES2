using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace LoginBancoTesteTests
{
    public static class WebServer
    {
        private static Process _iisProcess;
        private static String pathToProject = "";
        public static void StartIis()
        {
            if (_iisProcess == null)
            {
                var thread = new Thread(StartIisExpress) { IsBackground = true };
                thread.Start();
            }
        }
        private static void StartIisExpress()
        {
            var startInfo = new ProcessStartInfo { WindowStyle = ProcessWindowStyle.Normal, ErrorDialog = true, LoadUserProfile = true, CreateNoWindow = false, UseShellExecute = false, Arguments = string.Format("/path:\"{0}\" /port:{1}", @"D:\Documents\workspace\AspNet\Trabalho-ES2\LoginBancoTeste\obj\publish", "14577") };
            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"]) ? startInfo.EnvironmentVariables["programfiles(x86)"] : startInfo.EnvironmentVariables["programfiles"];
            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";
            try
            {
                _iisProcess = new Process { StartInfo = startInfo };
                _iisProcess.Start(); _iisProcess.WaitForExit();
            }
            catch
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }
        public static void StopIis()
        {
            if (_iisProcess != null)
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }
    }
}