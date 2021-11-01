using System;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Forms;
using System.Diagnostics;

namespace ShutdownManager
{
    internal enum eTimerZeroActions { Shutdown, Restart, EnergySafe }


    class ComputerFunctions
    {

        //Constanten
        private const string balloonTipTitle = "ShutdownManager";

        public TaskbarIcon tb = new TaskbarIcon();
        public Timer timer = new Timer();


        //variables
        private int hours;
        private int minutes;
        private int seconds;

        private bool timerHasStarted;


        //Properties
        public TimeSpan TimeLeft { get; set; }
        public eTimerZeroActions TimerZeroAction {get; set;}

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


        //Konstruktor
        public ComputerFunctions()
        {
            
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
        }




        //Methode

        private void UpdateTimeSpan()
        {
            TimeLeft = new TimeSpan(hours,minutes,seconds);
            
        }


        public void StartTimer()
        {
            if (!timerHasStarted)
            {
                string message = "Timer has started";

                BalloonIcon icon = BalloonIcon.Info;
                tb.ShowBalloonTip(balloonTipTitle, message, icon);

                timer.Start();
                timer.Enabled = true;
                timerHasStarted = true;
            }

        }

        public void StopTimer(bool withBalloonTip)
        {
            if (timerHasStarted)
            {
                if (withBalloonTip)
                {
                    string message = "Timer has stopped";
                    BalloonIcon icon = BalloonIcon.Info;
                    tb.ShowBalloonTip(balloonTipTitle, message, icon);
                }

                timer.Enabled = false;
                timer.Stop();

                timerHasStarted = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (TimeLeft.TotalSeconds < 1)
            {
                StopTimer(false);
                TimeLeft = new TimeSpan(0,0,0);
                TimerAktions();
            }
            
        }

        private void TimerAktions()
        {
            if(TimerZeroAction == eTimerZeroActions.Shutdown)
            {
                MessageBox.Show("Shutdown", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            }
            else if (TimerZeroAction == eTimerZeroActions.Restart)
            {
                MessageBox.Show("Restart");
            }
            else if (TimerZeroAction == eTimerZeroActions.EnergySafe)
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
