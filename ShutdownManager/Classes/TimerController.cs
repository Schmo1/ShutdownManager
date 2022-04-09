using System;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownManager.Utility;

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
            if (IsTimerStarted)
                return;
            if (!CheckIfSomeActionIsSelected()) //No action was selected
                return;
            
            timer.Start();
            timer.Enabled = true;
            IsTimerStarted = true;
            App.NotifyIconViewModel.SystemTrayMenuText = App.AppCon.RManager.GetString("timerActiv");
            App.ShowBalloonTip(App.AppCon.RManager.GetString("timerStarted"), BalloonIcon.Info);
            
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
                    App.NotifyIconViewModel.SetSystemTrayMenuTextToDefault();
                    if (WithMessage) { App.ShowBalloonTip(App.AppCon.RManager.GetString("timerStopped"), BalloonIcon.Info); }
                    
                }
                else
                {
                    if (WithMessage) {App.ShowBalloonTip(App.AppCon.RManager.GetString("timerPaused"), BalloonIcon.Info); }
                }
            }
            else //If Stop is only pressed => Reset TimeSpan
            {
                UpdateTimeSpan();
                App.NotifyIconViewModel.SetSystemTrayMenuTextToDefault();
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
                string message = App.AppCon.RManager.GetString("lastBaloonTip");


                if (App.ViewModel.ShutdownIsChecked)
                {
                    message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("shutdown").ToLower());
                }
                else if (App.ViewModel.RestartIsChecked)
                {
                    message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("restart").ToLower());
                }
                else if (App.ViewModel.SleepIsChecked)
                {
                    message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("sleep").ToLower());
                }

                App.ShowBalloonTip(message, BalloonIcon.Info);
            }
        }

        private void TimerAktions()
        { 
            if (App.ViewModel.ShutdownIsChecked)
            {
                ShutdownOptions.Instance.Shutdown();
            }
            else if (App.ViewModel.RestartIsChecked)
            {
                ShutdownOptions.Instance.Restart();
            }
            else
            {
                ShutdownOptions.Instance.Sleep(); 
            }
        
        }

        private bool CheckIfSomeActionIsSelected()
        {
            if (App.ViewModel.ShutdownIsChecked || App.ViewModel.RestartIsChecked || App.ViewModel.SleepIsChecked)
                return true;

            MessageBox.Show(App.AppCon.RManager.GetString("noActionSelected"), "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;

        }

        public void UpdateTimeSpan()
        {

            TimeSpan timeSpan = new TimeSpan(App.ViewModel.Hours, App.ViewModel.Minutes, App.ViewModel.Seconds);
            App.ViewModel.TimeSpanLeft = timeSpan;
            
        }
    }
}
