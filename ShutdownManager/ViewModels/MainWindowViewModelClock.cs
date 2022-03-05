

namespace ShutdownManager.ViewModels
{
    public partial class MainWindowViewModel
    {

        public string ClockTime { get { return _clockTime; } set { _clockTime = value; OnPropertyChanged(nameof(ClockTime)); } }

        public string TriggerTime { get { return _triggerTime; } set { _triggerTime = value; OnPropertyChanged(nameof(TriggerTime)); } }
        public int ClockHours
        {
            get => Properties.Settings.Default.ClockHours;
            set
            {
                Properties.Settings.Default.ClockHours = CheckMaxValue(23, value); ;
                SaveUserData(nameof(ClockHours));
                UpdateTriggerTime();
            }
        }
        public int ClockMinutes
        {

            get => Properties.Settings.Default.ClockMinutes;
            set
            {
                Properties.Settings.Default.ClockMinutes = CheckMaxValue(59, value); ;
                SaveUserData(nameof(ClockMinutes));
                UpdateTriggerTime();
            }
        }
        public int ClockSeconds
        {

            get => Properties.Settings.Default.ClockSeconds;
            set
            {
                Properties.Settings.Default.ClockSeconds = CheckMaxValue(59, value); ;
                SaveUserData(nameof(ClockSeconds));
                UpdateTriggerTime();
            }
        }

        public bool ShutdownClockIsChecked
        {
            get => Properties.Settings.Default.ShutdownClockIsChecked;
            set
            {
                Properties.Settings.Default.ShutdownClockIsChecked = value;
                SaveUserData(nameof(ShutdownClockIsChecked));
            }
        }
        public bool RestartClockIsChecked
        {
            get => Properties.Settings.Default.RestartClockIsChecked;
            set
            {
                Properties.Settings.Default.RestartClockIsChecked = value;
                SaveUserData(nameof(RestartClockIsChecked));    
            }
        }
        public bool SleepClockIsChecked
        {
            get => Properties.Settings.Default.SleepClockIsChecked;
            set
            {
                Properties.Settings.Default.SleepClockIsChecked = value;
                SaveUserData(nameof(SleepClockIsChecked));
            }
        }

        public bool IsClockObservingActiv
        {
            get { return _isClockObservingActiv; }
            set
            {
                _isClockObservingActiv = value;
                OnPropertyChanged(nameof(IsClockObservingActiv));
                App.ClockControl.IsClockObservingActiv = value;
            }
        }
    }
}
