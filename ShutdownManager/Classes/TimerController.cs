using System;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownManager.ViewModels;

namespace ShutdownManager.Classes
{

    public class TimerController
    {


        //Variables
        public Timer timer = new Timer();

        //Properties
        public bool IsTimerStarted { get => App.ViewModel.IsTimerStarted; set { App.ViewModel.IsTimerStarted = value; } }

        //Events
        public event EventHandler OnTimerIsOver;


        //Konstruktor
        public TimerController()
        {
            UpdateTimeSpan();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
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

        public void StopPauseTimer(bool isPaused, bool WithMessage)
        {
            if (IsTimerStarted)
            {
                timer.Enabled = false;
                timer.Stop();
                IsTimerStarted = false;

                if (!isPaused)//If the Timer is not paused, then Update the Timespan
                {
                    UpdateTimeSpan();
                    if (WithMessage) { App.TaskbarIcon.ShowBalloonTip("Info", "Timer has stopped", BalloonIcon.Info); }
                    
                }
                else
                {
                    if (WithMessage) {App.TaskbarIcon.ShowBalloonTip("Info", "Timer has paused", BalloonIcon.Info); }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            App.ViewModel.TimeSpanLeft = App.ViewModel.TimeSpanLeft.Subtract(TimeSpan.FromSeconds(1));

            if (App.ViewModel.TimeSpanLeft.TotalSeconds < 1)
            {
                OnTimerIsOver?.Invoke(this, EventArgs.Empty); //Event
                StopPauseTimer(false,false);
                App.ViewModel.TimeSpanLeft = new TimeSpan(0, 0, 0); //Sometimes the counter goes down to -1 
                UpdateTimeSpan();
                TimerAktions();
            }

            //Last Ballon tip 
            if (App.ViewModel.TimeSpanLeft.TotalSeconds == 60)
            {
                string message = string.Empty;


                if (App.ViewModel.ShutdownIsChecked)
                {
                    message = "PC will be shut down after 60 seconds!";
                }else if (App.ViewModel.RestartIsChecked)
                {
                    message = "PC will be restart after 60 seconds!";
                }else if (App.ViewModel.SleepIsChecked)
                {
                    message = "PC is going to sleep after 60 seconds!";
                }

                App.TaskbarIcon.ShowBalloonTip("Info", message, BalloonIcon.Info);
            }
        }

        private void TimerAktions()
        { 
            if (App.ViewModel.ShutdownIsChecked)
            {
                App.ShutdownOptions.Shutdown();
            }
            else if (App.ViewModel.RestartIsChecked)
            {
                App.ShutdownOptions.Restart();
            }
            else if (App.ViewModel.SleepIsChecked)
            {
                App.ShutdownOptions.Sleep(); 
            }
            else
            {
                MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void UpdateTimeSpan()
        {

            TimeSpan timeSpan = new TimeSpan(App.ViewModel.userDataPersistentManager.Hours, App.ViewModel.userDataPersistentManager.Minutes, App.ViewModel.userDataPersistentManager.Seconds);
            if (timeSpan > new TimeSpan(24, 0, 0))
            {
                App.ViewModel.TimeSpanLeft = new TimeSpan(23, 59, 59);
            }
            else
            {
                App.ViewModel.TimeSpanLeft = timeSpan;
            }

        }
    }
}
