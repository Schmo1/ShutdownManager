using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.CompilerServices;

namespace ShutdownManager.Classes
{

    public class TimerFunktionController : INotifyPropertyChanged
    {

        [DllImport("Powrprof.dll", SetLastError = true)]

        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);


        //Variables
        private bool _isTimerStarted;
        private string _timeLeft;
        private TimeSpan _timeSpanLeft;

        public Timer timer = new Timer();
        public UserDataPersistentManager userDataPersistentManager = new UserDataPersistentManager(); //Class to manage the UserData's


        //Properties
        public bool IsTimerStarted { get => _isTimerStarted; set { _isTimerStarted = value; OnPropertyChanged(); } }
        public TimeSpan TimeSpanLeft { get { return _timeSpanLeft; } set { _timeSpanLeft = value; TimeLeft = _timeSpanLeft.ToString(@"hh\:mm\:ss"); } }
        public string TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(); } }

        public bool ShutdownIsChecked { get => userDataPersistentManager.ShutdownIsChecked; set { userDataPersistentManager.ShutdownIsChecked = value; userDataPersistentManager.SaveUserData(); } }
        public bool RestartIsChecked { get => userDataPersistentManager.RestartIsChecked; set { userDataPersistentManager.RestartIsChecked = value; userDataPersistentManager.SaveUserData(); } }
        public bool SleepIsChecked { get => userDataPersistentManager.SleepIsChecked; set { userDataPersistentManager.SleepIsChecked = value; userDataPersistentManager.SaveUserData(); } }

        public int Hours
        {
            get => userDataPersistentManager.Hours;
            set
            {
                userDataPersistentManager.Hours = value;
                userDataPersistentManager.SaveUserData();
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
                userDataPersistentManager.SaveUserData();
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
                userDataPersistentManager.SaveUserData();
                UpdateTimeSpan();
                OnPropertyChanged();
            }
        }

        //Events
        public event EventHandler OnTimerIsOver;
        public event EventHandler OnTimerTick;
        public event PropertyChangedEventHandler PropertyChanged;


        //Konstruktor
        public TimerFunktionController()
        {
            userDataPersistentManager.LoadUserData();
            OnPropertyChanged();
            UpdateTimeSpan();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
        }



        //Methode

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
               Process.Start("shutdown", "/s /f /t 0");
            }
            else if (RestartIsChecked)
            { 
                Process.Start("shutdown", "/r /f /t 0");
            }
            else if (SleepIsChecked)
            {
               SetSuspendState(true, false, false); //With hibernate   
            }
            else
            {
                MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
