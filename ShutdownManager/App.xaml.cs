using System;
using System.Windows;
using ShutdownManager.Views;
using ShutdownManager.Classes;
using ShutdownManager.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownManager.Utility;

namespace ShutdownManager
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {

        //Variables
        private static TimerController _timerController;

        //Properties
        public static MainWindow Window { get; set; }
        private static SettingsView SettingsView { get; set; }
        public static TimerController TimerController { get => _timerController; set => _timerController = value; }
        public static MainWindowViewModel ViewModel { get; set; }   
        public static DownUploadController DownUploadController { get; set; }
        public static ClockControl ClockControl { get; set; }
        private static TaskbarIcon TaskbarIcon { get; set; }
        public static NotifyIconViewModel NotifyIconViewModel { get; set; }
        public static AppController AppCon { get; set; }
        public static ShutdownOptions ShutdownOptions { get; set; }
        public static bool HideWindowPressed { get; set; }  

        private delegate void OpenMainWindowDel();

       

        //Constructor

        public App()
        {
            MyLogger.GetInstance().Info("App is starting...");
            AppCon = new AppController();
            AppCon.OnOpenRequest += OpenGUIRequest; //Timer is over event
            DownUploadController = new DownUploadController();
            ClockControl = new ClockControl();
            ViewModel = new MainWindowViewModel();
            TimerController = new TimerController();
            NotifyIconViewModel = new NotifyIconViewModel();   
            ShutdownOptions = new ShutdownOptions();

        }



        //Methods

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            

            //if not first instance then exit App
            if (!AppCon.IsFirstInstance())
            {
                MyLogger.GetInstance().InfoWithClassName("App is not first instance ==> shutdown the application", Current);
                Current.Shutdown();
            }
            MyLogger.GetInstance().InfoWithClassName("App is the first instance", Current);

            ////start Listener
            AppCon.StartListening();

            try
            {
                //create the notifyicon (it's a resource declared in NotifyIconResources.xaml)
                TaskbarIcon = (TaskbarIcon)FindResource("Taskbar");
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().ErrorWithClassName("Error find Resource Taskbar. Exception " + ex.Message, Current);
            }

            if (WithUserInterface())
            {
               OpenMainWindow();
            }
            else
            {
                Window = new MainWindow();
            }

        }

        private void OpenGUIRequest(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new OpenMainWindowDel(OpenMainWindow));
        }



        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon.Dispose();
            DownUploadController.AbortThread();
            AppCon.StopListening();

            AppCon = null;
            MyLogger.GetInstance().Info("Close application 'OnExit'.... ");

            base.OnExit(e);
        }



        private bool WithUserInterface()
        {

            if (AppCon.OpenMinimized)
            {
                //Check if its on Startup
                string[] arguments = Environment.GetCommandLineArgs();

                bool WithUI = true;
                foreach (string arg in arguments)
                {
                    if (arg == "/startup")
                    {
                        MyLogger.GetInstance().InfoWithClassName($"Open UserInterface Found argument: {arg}", Current);
                        WithUI = false; 
                    }
                        
                    
                }
                MyLogger.GetInstance().InfoWithClassName($"Open UserInterface? {WithUI} ", Current);
                return WithUI;
            }
            else
            {
                MyLogger.GetInstance().InfoWithClassName("Open with UserInterface.Open minimized deaktivated", Current);
                return true;   
            }

        }

        public static void OpenMainWindow()
        {
            MyLogger.GetInstance().Info("Open main window");

            if (Window == null || Window.IsVisible == false)
            {
                try
                {
                    Window = new MainWindow();

                    //Create some events
                    Window.Closing += OnMainWindowClosing;

                    Window.Show();
                    Window.LoadInformations();
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().ErrorWithClassName("Open main window. Exception " + ex.Message, Current);
                }

            }

            Window.WindowState = WindowState.Normal;
        }


        public static void OpenSettings()
        {
            MyLogger.GetInstance().InfoWithClassName("Open settings", Current);

            if (SettingsView == null || SettingsView.IsVisible == false)
            {
                SettingsView = new SettingsView();
                try
                {
                    SettingsView.Show();
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().ErrorWithClassName("Open settings. Exception " + ex.Message, Current);
                }
            }

            SettingsView.WindowState = WindowState.Normal;
        }

        private static void OnMainWindowClosing(object source, EventArgs args)
        {

            MyLogger.GetInstance().InfoWithClassName("Closing main window", Current);

            Window.Closing -= OnMainWindowClosing;
            DownUploadController.AbortThread();

            if (TimerController.IsTimerStarted)
            {
                ShowBalloonTip("Info", "The timer is still running in the background", BalloonIcon.Info);
            }
            //Close Window only on X-Button
            else if (!AppCon.OnWindwoClosingActiv &! HideWindowPressed)
            {
                Current.Shutdown();
            }
            HideWindowPressed = false;

        }

        public static void ShowBalloonTip(string title, string message, BalloonIcon symbol )
        {

            if(symbol == BalloonIcon.Info)
                MyLogger.GetInstance().Info($"BalloonTip title: {title}, message: {message} ");
            else if(symbol == BalloonIcon.Warning)
                MyLogger.GetInstance().Warning($"BalloonTip title: {title}, message: {message} ");
            else
                MyLogger.GetInstance().Error($"BalloonTip title: {title}, message: {message} ");

            if (!AppCon.DisablePushMessages)
            {
                TaskbarIcon.ShowBalloonTip(title, message, symbol);  
            }
                
        }


    }
}
