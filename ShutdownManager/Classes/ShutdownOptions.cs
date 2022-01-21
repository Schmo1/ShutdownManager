
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShutdownManager.Classes
{
    public class ShutdownOptions
    {


        [DllImport("Powrprof.dll", SetLastError = true)]

        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        public void Shutdown()
        {
            Process.Start("shutdown", "/s /f /t 0");
        }

        public void Restart()
        {
            Process.Start("shutdown", "/r /f /t 0");
        }

        public void Sleep()
        {
            SetSuspendState(true, false, false); //With hibernate 
        }

    }
}
