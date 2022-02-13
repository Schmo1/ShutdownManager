using Hardcodet.Wpf.TaskbarNotification;
using ShutdownManager.Utility;
using System;
using System.Windows.Threading;

namespace ShutdownManager.Classes
{

    public class ClockControl
    {

        private DateTime _time;
        DispatcherTimer _timer = new DispatcherTimer();


        public bool IsClockObservingActiv { get => App.ViewModel.IsClockObservingActiv; }
        public DateTime TriggerTime 
        { 
            get 
            { return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)(App.ViewModel?.ClockHours), (int)App.ViewModel?.ClockMinutes, (int)App.ViewModel?.ClockSeconds); } 
        }
        public DateTime ClockTime { get { return _time; } }

        public DispatcherTimer Timer { get { return _timer; } set { _timer = value; } }

        public ClockControl()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Update_Timer;
            _timer.Start();
        }


        private void Update_Timer(object sender, EventArgs e)
        {
            _time = DateTime.Now;

            if (!IsClockObservingActiv) { return; }


            //Check time is over/equal
            if (CheckTimeEqual(_time, TriggerTime))
            {
                MyLogger.GetInstance().InfoWithClassName($"ClockTime is over Time: {_time.TimeOfDay}", this);

                if (App.ViewModel.ShutdownClockIsChecked)
                {
                    App.ShutdownOptions.Shutdown();
                }
                else if (App.ViewModel.SleepClockIsChecked)
                {
                    App.ShutdownOptions.Sleep();
                }
                else
                {
                    App.ShutdownOptions.Restart();
                }
            }else if(CheckTimeEqual(_time, TriggerTime.AddMinutes(-1)))
            {
                //Message wehn only 60s left before action
                CreateBaloonTip();
            }
        }

        private void CreateBaloonTip()
        {
            string message = "PC is going to XXReplaceTemplate in 60 seconds!";
            if (App.ViewModel.ShutdownClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplate", "shut down");
            }
            else if (App.ViewModel.SleepClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplate", "sleep");
            }
            else
            {
                message = message.Replace("XXReplaceTemplate", "restart");
            }

            App.ShowBalloonTip("Info", message, BalloonIcon.Info);
        }

        private bool CheckTimeEqual(DateTime dateTime1, DateTime dateTime2)
        {
            if(dateTime1.Hour == dateTime2.Hour && dateTime1.Minute == dateTime2.Minute && dateTime1.Second == dateTime2.Second)
            {
                return true;
            }
            return false;
        }

    }
}
