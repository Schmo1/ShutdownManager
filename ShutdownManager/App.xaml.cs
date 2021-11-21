using System;
using System.Windows;
using System.Threading;
using System.IO;
using System.IO.MemoryMappedFiles;
using ShutdownManager.Views;
using ShutdownManager.Classes;
using ShutdownManager.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownManager
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {

        //Variables
        private static TimerFunktionController _timerFunktionController;

        //Properties
        public static MainWindow Window { get; set; }
        public static TimerFunktionController TimerFunktionController { get => _timerFunktionController; set => _timerFunktionController = value; }
        public static TaskbarIcon TaskbarIcon { get; set; }
        public static NotifyIconViewModel NotifyIconViewModel { get; set; }
        public static AppController AppCon { get; set; }

        private delegate void OpenMainWindowDel();

       

        //Constructor

        public App()
        {

            AppCon = new AppController();
            AppCon.OnOpenRequest += OpenGUIRequest; //Timer is over event
            
            TimerFunktionController = new TimerFunktionController();
            NotifyIconViewModel = new NotifyIconViewModel();
            Window = new MainWindow();

        }



        //Methods

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);


            //if not first instance then exit App
            if(!AppCon.IsFirstInstance())
            {
               Current.Shutdown();
            }

            ////start Listener
            AppCon.StartListening();

            try
            {
                //create the notifyicon (it's a resource declared in NotifyIconResources.xaml)
                TaskbarIcon = (TaskbarIcon)FindResource("Taskbar");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Find Resource", MessageBoxButton.OK ,MessageBoxImage.Error);
            }
           

            if (WithUserInterface())
            {
                OpenMainWindow();
            }
            
        }

        private void OpenGUIRequest(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new OpenMainWindowDel(OpenMainWindow));
        }



        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon.Dispose();

            AppCon.StopListening();
            AppCon = null;

            base.OnExit(e);
        }



        private bool WithUserInterface()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            bool WithUi = true;
            foreach (string arg in arguments)
            {
                if (arg == "/ui") { WithUi = false; }
            }

            return WithUi;
        }

        public static void OpenMainWindow()
        {
            if (Window == null || Window.IsVisible == false)
            {
                try
                {
                    Window = new MainWindow();
                    Window.Closing += OnMainWindowClosing;
                    Window.Show();
                    Window.LoadInformations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Create MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            Window.WindowState = WindowState.Normal;
        }

        private static void OnMainWindowClosing(object source, EventArgs args)
        {
            if (TimerFunktionController.IsTimerStarted)
            {
                TaskbarIcon.ShowBalloonTip("Info", "The timer is still running in the background", BalloonIcon.Info);
            }

            Window.Closing -= OnMainWindowClosing;
        }


    }
}
