using System;
using System.Windows.Forms;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownManager.Utility;

namespace ShutdownManager.Classes
{

    public class ClockControl
    {

        private DateTime _time;
        private bool _isClockObservingActiv;
        DispatcherTimer _timer = new DispatcherTimer();


        public bool IsClockObservingActiv 
        { 
            get { return _isClockObservingActiv; } 
            set 
            { 
                _isClockObservingActiv = CheckIfSomeActionIsSelected() && value;
                if (_isClockObservingActiv)
                {
                    CreateClockObservingOnTip();
                    App.NotifyIconViewModel.SystemTrayMenuText = App.AppCon.RManager.GetString("clockTriggerIsActiv");
                }
                else
                    App.NotifyIconViewModel.SetSystemTrayMenuTextToDefault();
            } 
        }
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
                MyLogger.GetInstance().DebugWithClassName($"ClockTime is over Time: {_time.TimeOfDay}", this);

                if (App.ViewModel.ShutdownClockIsChecked)
                {
                    ShutdownOptions.Instance.Shutdown();
                }
                else if (App.ViewModel.SleepClockIsChecked)
                {
                    ShutdownOptions.Instance.Sleep();
                }
                else
                {
                    ShutdownOptions.Instance.Restart();
                }
            }else if(CheckTimeEqual(_time, TriggerTime.AddMinutes(-1)))
            {
                //Message wehn only 60s left before action
                CreateLastBaloonTip();
            }
        }

        private void CreateClockObservingOnTip()
        {
            
            string message = App.AppCon.RManager.GetString("createClockObservingMessage"); ;
            if (App.ViewModel.ShutdownClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("shutdown").ToLower());
            }
            else if (App.ViewModel.SleepClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("sleep").ToLower());
            }
            else
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("restart").ToLower());
            }

            string remainingTime;

            if (TriggerTime > _time)
                remainingTime = (TriggerTime - _time).ToString(@"hh\:mm\:ss");
            else
                remainingTime = (TriggerTime + new TimeSpan(24, 0, 0) - _time).ToString(@"hh\:mm\:ss");

            message = message.Replace("XXReplaceTemplateTimeXX", remainingTime);



            App.ShowBalloonTip(message, BalloonIcon.Info);
        }


        private void CreateLastBaloonTip()
        {
            string message = App.AppCon.RManager.GetString("lastBaloonTip");
            if (App.ViewModel.ShutdownClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("shutdown").ToLower());
            }
            else if (App.ViewModel.SleepClockIsChecked)
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("sleep").ToLower());
            }
            else
            {
                message = message.Replace("XXReplaceTemplateXX", App.AppCon.RManager.GetString("restart").ToLower());
            }

            App.ShowBalloonTip(message, BalloonIcon.Info);
        }

        private bool CheckTimeEqual(DateTime dateTime1, DateTime dateTime2)
        {
            if(dateTime1.Hour == dateTime2.Hour && dateTime1.Minute == dateTime2.Minute && dateTime1.Second == dateTime2.Second)
            {
                return true;
            }
            return false;
        }

        private bool CheckIfSomeActionIsSelected()
        {
            if (App.ViewModel.ShutdownClockIsChecked || App.ViewModel.RestartClockIsChecked || App.ViewModel.SleepClockIsChecked)
                return true;

            MessageBox.Show(App.AppCon.RManager.GetString("noActionSelected"), "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;

        }

    }
}
