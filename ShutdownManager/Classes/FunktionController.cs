using System;
using System.Windows.Forms;
using System.ComponentModel;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.CompilerServices;

namespace ShutdownManager.Classes
{

    public class FunktionController : INotifyPropertyChanged
    {


        //Variables
        private bool _isTimerStarted;
        private string _timeLeft;
        private TimeSpan _timeSpanLeft;
        private string _uploadValue;
        private string _downloadValue;

        public Timer timer = new Timer();
        public UserDataPersistentManager userDataPersistentManager = new UserDataPersistentManager(); //Class to manage the UserData's



        //Properties
        public bool IsTimerStarted { get => _isTimerStarted; set { _isTimerStarted = value; OnPropertyChanged(); } }
        public TimeSpan TimeSpanLeft { get { return _timeSpanLeft; } set { _timeSpanLeft = value; TimeLeft = _timeSpanLeft.ToString(@"hh\:mm\:ss"); } }
        public string TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(); } }
        


        //Timer Functions
        public bool ShutdownIsChecked { get => userDataPersistentManager.ShutdownIsChecked; set { userDataPersistentManager.ShutdownIsChecked = value;} }
        public bool RestartIsChecked { get => userDataPersistentManager.RestartIsChecked; set { userDataPersistentManager.RestartIsChecked = value;} }
        public bool SleepIsChecked { get => userDataPersistentManager.SleepIsChecked; set { userDataPersistentManager.SleepIsChecked = value;} }


        // Down- Upload Functions
        public bool DownloadIsChecked
        {
        get => userDataPersistentManager.DownloadIsChecked;
            set
            {
                userDataPersistentManager.DownloadIsChecked = value;
                App.DownUploadController.ObserveFunction = DownUploadController.LoadFunction.Download;
            }
        }


        public bool UploadIsChecked
        {
            get => userDataPersistentManager.UploadIsChecked;
            set
            {
                userDataPersistentManager.UploadIsChecked = value;
                App.DownUploadController.ObserveFunction = DownUploadController.LoadFunction.Upload;
            }
        }



        //Times for Timerfunctioncontrol
        public int Hours
        {
            get => userDataPersistentManager.Hours;
            set
            {
                userDataPersistentManager.Hours = value;
                UpdateTimeSpan();
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get => userDataPersistentManager.Minutes;
            set
            {
                userDataPersistentManager.Minutes = value;
                UpdateTimeSpan();
                OnPropertyChanged();
            }
        }

        public int Seconds
        {
            get => userDataPersistentManager.Seconds;
            set
            {
                userDataPersistentManager.Seconds = value;
                UpdateTimeSpan();
                OnPropertyChanged();
            }
        }


        public bool IsObserveActiv { get => App.DownUploadController.IsObserveActiv; set => App.DownUploadController.IsObserveActiv = value; }    

        //Down- Upload Control
        public int ObserveTime
        {
            get => userDataPersistentManager.ObserveTime;
            set
            { 
                int maxValue = 99999;
                int minValue = 2;
                if (value > maxValue)
                {
                    userDataPersistentManager.ObserveTime = maxValue;
                }else if (value < minValue)
                {
                    userDataPersistentManager.ObserveTime = minValue;
                }
                else
                {
                    userDataPersistentManager.ObserveTime = value;
                }
                App.DownUploadController.ObserveTime = userDataPersistentManager.ObserveTime;

            }
        }
        public double Speed
        {
            get => userDataPersistentManager.Speed;
            set
            {
                userDataPersistentManager.Speed = value;
            }
        }

        public string DownloadValue
        {
            get => _downloadValue;
            set
            {
                _downloadValue = value;
                OnPropertyChanged();
            }
        }

        public string UploadValue {
            get => _uploadValue;
            set
            {
                _uploadValue = value;
                OnPropertyChanged();
            }
        }

        public bool DownUploadIsSelected 
        {
            get => App.DownUploadController.IsViewActiv;
            set => App.DownUploadController.IsViewActiv = value;
        }



        //Events
        public event EventHandler OnTimerIsOver;
        public event PropertyChangedEventHandler PropertyChanged;


        //Konstruktor
        public FunktionController()
        {
            userDataPersistentManager.LoadUserData();
            OnPropertyChanged();
            UpdateTimeSpan();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
            CheckEmptyUserInput();
        }



        //Methode

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void CheckEmptyUserInput()
        {
            if(ObserveTime == 0)
            {
                ObserveTime = 10;
            }

            if(Speed == 0)
            {
                Speed = 0.5;
            }

            if(!DownloadIsChecked && !UploadIsChecked)
            {
                DownloadIsChecked = true;
            }
        }
        public void UpdateTimeSpan()
        {

            TimeSpan timeSpan = new TimeSpan(userDataPersistentManager.Hours, userDataPersistentManager.Minutes, userDataPersistentManager.Seconds);
            if (timeSpan > new TimeSpan(24, 0, 0))
            {
                TimeSpanLeft = new TimeSpan(23, 59, 59);
            }
            else
            {
                TimeSpanLeft = timeSpan;
            }

        }

        public void StartTimer()
        {
            if (!IsTimerStarted)
            {
                timer.Start();
                timer.Enabled = true;
                IsTimerStarted = true;
                App.TaskbarIcon.ShowBalloonTip("Info", "Timer has started", BalloonIcon.Info);
            }
        }

        public void StopPauseTimer(bool isPaused)
        {
            if (IsTimerStarted)
            {
                timer.Enabled = false;
                timer.Stop();
                IsTimerStarted = false;

                if (!isPaused)//If the Timer is not paused, then Update the Timespan
                {
                    UpdateTimeSpan();
                    App.TaskbarIcon.ShowBalloonTip("Info", "Timer has stopped", BalloonIcon.Info);
                }
                else
                {
                    App.TaskbarIcon.ShowBalloonTip("Info", "Timer has paused", BalloonIcon.Info);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpanLeft = TimeSpanLeft.Subtract(TimeSpan.FromSeconds(1));

            if (TimeSpanLeft.TotalSeconds < 1)
            {
                OnTimerIsOver?.Invoke(this, EventArgs.Empty); //Event
                StopPauseTimer(false);
                TimeSpanLeft = new TimeSpan(0, 0, 0); //Sometimes the counter goes down to -1 
                UpdateTimeSpan();
                TimerAktions();
            }

            //Last Ballon tip 
            if (TimeSpanLeft.TotalSeconds == 60)
            {
                string message = string.Empty;


                if (ShutdownIsChecked)
                {
                    message = "PC will be shut down after 60 seconds!";
                }else if (RestartIsChecked)
                {
                    message = "PC will be restart after 60 seconds!";
                }else if (SleepIsChecked)
                {
                    message = "PC is going to sleep after 60 seconds!";
                }

                App.TaskbarIcon.ShowBalloonTip("Info", message, BalloonIcon.Info);
            }
        }

        private void TimerAktions()
        { 
            if (ShutdownIsChecked)
            {
                App.ShutdownOptions.Shutdown();
            }
            else if (RestartIsChecked)
            {
                App.ShutdownOptions.Restart();
            }
            else if (SleepIsChecked)
            {
                App.ShutdownOptions.Sleep(); 
            }
            else
            {
                MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
