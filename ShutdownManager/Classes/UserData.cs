

namespace ShutdownManager.Classes
{
    public class UserData
    {
        //Proberties

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public bool ShutdownIsChecked { get; set; }
        public bool RestartIsChecked { get; set; }
        public bool SleepIsChecked { get; set; }



        //Constructors
        public UserData(int hours, int minutes, int seconds, bool shutdownIsChecked, bool restartIsChecked, bool sleepIsChecked)
        {

            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            ShutdownIsChecked = shutdownIsChecked;
            RestartIsChecked = restartIsChecked;
            SleepIsChecked = sleepIsChecked;

        }
        public UserData()
        {

        }
    }
}
