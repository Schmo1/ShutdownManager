using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace ShutdownManager
{
    internal enum ETimerActions { Shutdown, Restart, EnergySafe }

    internal class TimerFunktionController
    {

        public Timer timer = new Timer();


        //Konstruktor
        public TimerFunktionController()
        {
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
        }

        //variables
        private int hours;
        private int minutes;
        private int seconds;

        private bool timerHasStarted;


        //Properties
        public bool TimerHasStarted => timerHasStarted;

        public TimeSpan TimeLeft { get; set; }
        public ETimerActions TimerAction {get; set;}

        public int Hours
        {
            get => hours;
            set
            {
                hours = value;
                UpdateTimeSpan();
            }
        }

        public int Minutes
        {
            get => minutes;
            set
            {
                minutes = value;
                UpdateTimeSpan();
            }
        }

        public int Seconds
        {
            get => seconds;
            set
            {
                seconds = value;
                UpdateTimeSpan();
            }
        }


        //Events
        public event EventHandler OnTimerIsOver;


        //Methode

        private void UpdateTimeSpan()
        {
            TimeLeft = new TimeSpan(hours,minutes,seconds);
        }


        public void StartTimer()
        {
            if (!timerHasStarted)
            {
                timer.Start();
                timer.Enabled = true;
                timerHasStarted = true;
            }
        }

        public void StopPauseTimer(bool isPaused)
        {
            if (timerHasStarted)
            {
                
                timer.Enabled = false;
                timer.Stop();
                timerHasStarted = false;

                if (!isPaused)//If the Timer is not paused, then Update the Timespan
                {
                    UpdateTimeSpan();
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (TimeLeft.TotalSeconds < 1)
            {
                OnTimerIsOver?.Invoke(this, EventArgs.Empty); //Event
                StopPauseTimer(false);
                TimeLeft = new TimeSpan(0, 0, 0); //Sometimes the counter goes down to -1 
                UpdateTimeSpan();
                TimerAktions();
            }
            
        }

        private void TimerAktions()
        {
            if(TimerAction == ETimerActions.Shutdown)
            {
                MessageBox.Show("Shutdown", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            }
            else if (TimerAction == ETimerActions.Restart)
            {
                MessageBox.Show("Restart");
            }
            else if (TimerAction == ETimerActions.EnergySafe)
            {
                MessageBox.Show("EnergySafe");
            }
            else
            {
                MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }




    
}
