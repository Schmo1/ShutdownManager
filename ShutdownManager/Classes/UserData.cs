using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutdownManager.Classes
{
    class UserData
    {
        //Proberties

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public ETimerActions TimerAction { get; set; }


        //Constructors
        public UserData(int hours, int minutes, int seconds, ETimerActions timerActions)
        {

            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            TimerAction = timerActions;

        }
        public UserData()
        {

        }
    }
}
