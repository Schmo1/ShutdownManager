using System;
using System.Windows.Threading;

namespace ShutdownManager.Classes
{

    public class ClockControl
    {

        private DateTime _time;
        DispatcherTimer _timer = new DispatcherTimer();


        public bool IsClockObservingActiv { get; set; }
        public DateTime ObservingTime { get; set; }
        public DateTime ClockTime { get { return _time; } }

        public DispatcherTimer Timer { get { return _timer; } set { _timer = value; } }

        public ClockControl()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Update_Timer;
            _timer.Start();
        }


        void Update_Timer(object sender, EventArgs e)
        {
            _time = DateTime.Now;

            if (IsClockObservingActiv && (ObservingTime == _time))
            {
                //do some action
            }
        }
    }
}
