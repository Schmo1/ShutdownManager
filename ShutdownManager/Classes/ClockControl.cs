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
  


            if (IsClockObservingActiv && CheckTimeEqual(_time, TriggerTime))
            {
                MyLogger.GetInstance().InfoWithClassName($"ClockTime is over Time: {_time.TimeOfDay}", this);
                //do some action
            }
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
