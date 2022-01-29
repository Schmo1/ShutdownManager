using Microsoft.Win32;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using Forms = System.Windows.Forms;

namespace ShutdownManager.Classes
{
    public class AppController
    {

        private static Mutex mutex;
        private static Thread thReciver;
        private static bool _firstInstance;
        private static bool _isAutoStartChecked;
        private string appName = Application.ResourceAssembly.GetName().Name;


        public string AppName { get { return appName; } }

        public bool IsAutoStartChecked  {get {return _isAutoStartChecked; }}

        public event EventHandler OnOpenRequest;

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);



        public AppController()
        {
            mutex = new Mutex(false, appName + "BySchmo", out _firstInstance);
            thReciver = new Thread(ReciveRequestForGUI);
            thReciver.SetApartmentState(ApartmentState.STA);

            CheckRegistry();

        }

        private void CheckRegistry()
        {
            object objValue = rkApp.GetValue(appName);

            // Check to see the current state (running at startup or not)
            if (objValue == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                _isAutoStartChecked = false;
            }
            else
            {
                // The value exists, the application is set to run at startup
                _isAutoStartChecked = true;
                try
                {
                    //adjust Path if its changed
                    string dirPath = CreateRegPath();

                    if (objValue.ToString() != dirPath)
                    {
                        rkApp.SetValue(appName, dirPath);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error GetValue", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }


        public void ChangeAutoStartChecked()
        {
            if (_isAutoStartChecked)
            {
                try
                {
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue(appName, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error Delete Registry entry", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _isAutoStartChecked = false;
            }
            else
            {
                try
                {
                    //Get Full Path
                    string dirPath = CreateRegPath();

                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue(appName, dirPath);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Argument NullException", "Error Appcontroller", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error GetFullPath", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _isAutoStartChecked = true;
            }

        }

        private string CreateRegPath()
        {
            //Get Full Path
            string dirInfo = Path.GetFullPath(Forms.Application.ExecutablePath);
            //add quotation marks and argument
            dirInfo = $"\"{dirInfo}\"" + " /ui";

            return dirInfo;
        }


        public void StartListening()
        {
            try
            {
                if (!thReciver.IsAlive) { thReciver.Start(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error Threading", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReciveRequestForGUI()
        {
            //if instance of the programm has started, this instance will open the GUI and the other instance closes itself.
            using (var mmf = MemoryMappedFile.CreateOrOpen("ShowGuiMapName", 1024))
            using (var view = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(view);
                EventWaitHandle signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ListenForOpenGUI");


                while (true)
                {
                    signal.WaitOne();
                    mutex.WaitOne();
                    reader.BaseStream.Position = 0;
                    if (reader.ReadString() == "OpenGUI")
                    {
                        OnOpenRequest?.Invoke(this, EventArgs.Empty); //Event
                    }
                    mutex.ReleaseMutex();
                }
            }
        }



        public bool IsFirstInstance()
        {
            try
            {

                if (!_firstInstance)
                {
                    //Send request to 
                    SendGUIRequest();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error Mutex", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return true;
        }

        public void SendGUIRequest()
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("ShowGuiMapName", 1024))
            using (var view = mmf.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(view);
                EventWaitHandle signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ListenForOpenGUI");

                string message = "OpenGUI";

                mutex.WaitOne();
                writer.BaseStream.Position = 0;
                writer.Write(message);
                signal.Set();
                mutex.ReleaseMutex();

                Thread.Sleep(1000);

            }
        }

        public void StopListening()
        {
            thReciver.Abort();
        }

        ~AppController()
        {
            thReciver.Abort();
        }

    }
}
