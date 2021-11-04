using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ShutdownManager.Classes;

namespace ShutdownManager
{
    internal enum ETimerActions { Shutdown, Restart, Sleep}
    

    internal class TimerFunktionController
    {

        [DllImport("Powrprof.dll", SetLastError = true)]
        
        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        public Timer timer = new Timer();
        public UserDataPersistentManager userDataPersistentManager = new UserDataPersistentManager(); //Class to manage the UserData's

        //Konstruktor
        public TimerFunktionController()
        {

            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000; //one Second  
        }

        //Constants

        private const bool isTestingMode = true;

        //Variables
        private bool timerHasStarted;


        //Properties
        public bool TimerHasStarted => timerHasStarted;

        public TimeSpan TimeLeft { get; set; }
        public ETimerActions TimerAction { get => userDataPersistentManager.TimerAction; set { userDataPersistentManager.TimerAction = value; userDataPersistentManager.SaveUserData(); } }

        public int Hours
        {
            get => userDataPersistentManager.Hours;
            set
            {
                userDataPersistentManager.Hours = value;
                UpdateTimeSpan();
                userDataPersistentManager.SaveUserData();
            }
        }

        public int Minutes
        {
            get => userDataPersistentManager.Minutes;
            set
            {
                userDataPersistentManager.Minutes = value;
                UpdateTimeSpan();
                userDataPersistentManager.SaveUserData();
            }
        }

        public int Seconds
        {
            get => userDataPersistentManager.Seconds;
            set
            {
                userDataPersistentManager.Seconds = value;
                UpdateTimeSpan();
                userDataPersistentManager.SaveUserData();
            }
        }


        //Events
        public event EventHandler OnTimerIsOver;


        //Methode

        public void UpdateTimeSpan()
        {
            TimeLeft = new TimeSpan(userDataPersistentManager.Hours, userDataPersistentManager.Minutes, userDataPersistentManager.Seconds);
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
            if (isTestingMode)
            {
                if (userDataPersistentManager.TimerAction == ETimerActions.Shutdown)
                {
                    MessageBox.Show("Shutdown", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else if (userDataPersistentManager.TimerAction == ETimerActions.Restart)
                {
                    MessageBox.Show("Restart");
                }                
                else if (userDataPersistentManager.TimerAction == ETimerActions.Sleep)
                {
                    MessageBox.Show("Sleep");
                }
                else
                {
                    MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (userDataPersistentManager.TimerAction == ETimerActions.Shutdown)
                {
                    Process.Start("shutdown", "/s /f /t 0");
                }
                else if (userDataPersistentManager.TimerAction == ETimerActions.Restart)
                {
                    Process.Start("shutdown", "/r /f /t 0");
                }
                else if (userDataPersistentManager.TimerAction == ETimerActions.Sleep)
                {
                    SetSuspendState(false, false, false);
                }
                else
                {
                    MessageBox.Show("No action was selected. Please select some action!", "Invalid action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

    }
}
