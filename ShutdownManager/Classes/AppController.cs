using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;

namespace ShutdownManager.Classes
{
    public class AppController
    {

        private static Mutex mutex;
        private static Thread thReciver;
        private static bool _firstInstance;
        private readonly string appName = Application.ResourceAssembly.GetName().Name;


        public string AppName { get { return appName; } }
        public AutoStart AutoStart { get; set; }

        public bool OnWindwoClosingActiv { get => Properties.Settings.Default.OnWindowClosingActiv; }
        public bool OpenMinimized { get => Properties.Settings.Default.OpenMinimized; }
        public bool DisablePushMessages {  get => Properties.Settings.Default.DisablePushMessages; }

        public event EventHandler OnOpenRequest;



        public AppController()
        {
            mutex = new Mutex(false, appName + "BySchmo", out _firstInstance);
            thReciver = new Thread(ReciveRequestForGUI);
            thReciver.SetApartmentState(ApartmentState.STA);
            AutoStart = new AutoStart(" /startup");

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
