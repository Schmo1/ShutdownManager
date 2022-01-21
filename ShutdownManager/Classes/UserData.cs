

namespace ShutdownManager.Classes
{
    public class UserData
    {

        /// <summary>
        //Proberties
        /// </summary>

        //Timer Control
        public virtual int Hours { get; set; }
        public virtual int Minutes { get; set; }
        public virtual int Seconds { get; set; }
        public virtual bool ShutdownIsChecked { get; set; }
        public virtual bool RestartIsChecked { get; set; }
        public virtual bool SleepIsChecked { get; set; }

        //Down- or Upload Control
        public virtual double Speed{ get; set; }
        public virtual int ObserveTime { get; set; }
        public virtual bool DownloadIsChecked { get; set; }
        public virtual bool UploadIsChecked { get; set; }




        //Constructors
        public UserData(int hours, int minutes, int seconds, bool shutdownIsChecked, bool restartIsChecked, bool sleepIsChecked, double speed, int observeTime, bool downloadIsChecked, bool uploadIsChecked)
        {

            //Timer Control
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            ShutdownIsChecked = shutdownIsChecked;
            RestartIsChecked = restartIsChecked;
            SleepIsChecked = sleepIsChecked;

            //Down- or Upload Control
            Speed = speed;
            ObserveTime = observeTime;
            DownloadIsChecked = downloadIsChecked;
            UploadIsChecked = uploadIsChecked;

        }
        public UserData()
        {

        }
    }
}
