using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShutdownManager.Classes
{
    public class ShutdownOptions
    {


        [DllImport("Powrprof.dll", SetLastError = true)]

        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        private bool _testingModeActiv;

        public ShutdownOptions()
        {
#if DEBUG
            _testingModeActiv = true;
#else
             _testingModeActiv = false;
#endif
        }

        public void Shutdown()
        {
            if (_testingModeActiv)
            {
                MessageBox.Show("Shutdown!");
            }
            else
            {
                Process.Start("shutdown", "/s /f /t 0");
            }
            
        }

        public void Restart()
        {
            if (_testingModeActiv)
            {
                MessageBox.Show("Restart!");
            }
            else
            {
                Process.Start("shutdown", "/r /f /t 0");
            }
        }

        public void Sleep()
        {
            if (_testingModeActiv)
            {
                MessageBox.Show("Sleep!");
            }
            else
            {
                SetSuspendState(true, false, false); //With hibernate 
            }
           
        }

    }
}
