using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShutdownManager.Commands;
using ShutdownManager.Utility;

namespace ShutdownManager.Classes
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {


        //Variables

        private string _timeLeft;
        private TimeSpan _timeSpanLeft;
        private string _uploadValue;
        private string _downloadValue;
        private bool _isTimerStarted;
        private bool _isClockObservingActiv;
        private ICommand _clickCommand;
        private string _clockTime;
        private string _triggerTime;


        public ICommand TriggerNowCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => NowPressed(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public void NowPressed()
        {
            if (ShutdownIsChecked)
            {
                ShutdownOptions.Instance.Shutdown();
            }
            else if (RestartIsChecked)
            {
                ShutdownOptions.Instance.Restart();
            }
            else
            {
                ShutdownOptions.Instance.Sleep();
            }
        }



        //Properties
        public TimeSpan TimeSpanLeft { get { return _timeSpanLeft; } set { _timeSpanLeft = value; TimeLeft = _timeSpanLeft.ToString(@"hh\:mm\:ss"); } }
        public string TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(nameof(TimeLeft)); ; } }
        public bool IsTimerStarted { get { return _isTimerStarted; } set { _isTimerStarted = value; OnPropertyChanged(nameof(IsTimerStarted)); } }



        //Timer Functions
        public bool ShutdownIsChecked { get => Properties.Settings.Default.TimerShutdownIsChecked; set { Properties.Settings.Default.TimerShutdownIsChecked = value; Properties.Settings.Default.Save(); } }
        public bool RestartIsChecked { get => Properties.Settings.Default.TimerRestartIsChecked; set { Properties.Settings.Default.TimerRestartIsChecked = value;  Properties.Settings.Default.Save(); } }
        public bool SleepIsChecked { get => Properties.Settings.Default.TimerSleepIsChecked; set { Properties.Settings.Default.TimerSleepIsChecked = value; Properties.Settings.Default.Save(); } }

        //DownUpload Functions
        public bool ShutdownIsCheckedDownUP { get => Properties.Settings.Default.ShutdownIsCheckedDownUP; set { Properties.Settings.Default.ShutdownIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }
        public bool RestartIsCheckedDownUP { get => Properties.Settings.Default.RestartIsCheckedDownUP; set { Properties.Settings.Default.RestartIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }
        public bool SleepIsCheckedDownUP { get => Properties.Settings.Default.SleepIsCheckedDownUP; set { Properties.Settings.Default.SleepIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }



        // Down- Upload Functions
        public bool DownloadIsChecked { get => Properties.Settings.Default.DownloadIsChecked; set { Properties.Settings.Default.DownloadIsChecked = value; Properties.Settings.Default.Save(); } }

        public bool UploadIsChecked { get => Properties.Settings.Default.UploadIsChecked; set { Properties.Settings.Default.UploadIsChecked = value; Properties.Settings.Default.Save(); } }



        //Times for Timerfunctioncontrol
        public int Hours
        {
            get => Properties.Settings.Default.TimerHours;
            set
            {
                Properties.Settings.Default.TimerHours = CheckMaxValue(23, value);
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged(nameof(Hours));
            }
        }

        public int Minutes
        {
            get => Properties.Settings.Default.TimerMinutes;
            set
            {
                Properties.Settings.Default.TimerMinutes = CheckMaxValue(59, value); ;
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged(nameof(Minutes));
            }
        }

        public int Seconds
        {
            get => Properties.Settings.Default.TimerSeconds;
            set
            {
                Properties.Settings.Default.TimerSeconds = CheckMaxValue(59, value); ;
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged(nameof(Seconds));
            }
        }


        public bool IsObserveDownUploadActiv { get { return App.DownUploadController.IsObserveActiv; } set { App.DownUploadController.IsObserveActiv = value; OnPropertyChanged(nameof(IsObserveDownUploadActiv)); }  }



        //Down- Upload Control
        public int ObserveTime
        {
            get => Properties.Settings.Default.ObserveTime;
            set
            {
                int minValue = 2;

                if (CheckMaxValue(99999, value) < minValue)
                {
                    Properties.Settings.Default.ObserveTime = minValue;
                }
                else
                {
                    Properties.Settings.Default.ObserveTime = value;
                }
                Properties.Settings.Default.Save();
                App.DownUploadController.ObserveTime = Properties.Settings.Default.ObserveTime;

            }
        }
        public double Speed
        {
            get => Properties.Settings.Default.Speed;
            set
            {
                double maxValue = 1000;
                double minValue = 0.1;

                if (value > maxValue)
                {
                    Properties.Settings.Default.Speed = maxValue;
                }
                else if (value < minValue)
                {
                    Properties.Settings.Default.Speed = minValue;
                }
                else
                {
                    Properties.Settings.Default.Speed = value;
                }
                Properties.Settings.Default.Save();
            }
        }

        public string DownloadValue
        {
            get => _downloadValue;
            set
            {
                _downloadValue = value;
                OnPropertyChanged(nameof(DownloadValue));
            }
        }

        public string UploadValue
        {
            get => _uploadValue;
            set
            {
                _uploadValue = value;
                OnPropertyChanged(nameof(UploadValue));
            }
        }

        public bool DownUploadIsSelected
        {
            get => App.DownUploadController.IsTapActiv;
            set => App.DownUploadController.IsTapActiv = value;
        }


        public string ClockTime { get { return _clockTime; } set { _clockTime = value; OnPropertyChanged(nameof(ClockTime)); }}

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
        public int ClockSeconds {

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







        //Events
        public event PropertyChangedEventHandler PropertyChanged;


        //Konstruktor
        public MainWindowViewModel()
        {
            OnPropertyChanged();
            CheckEmptyUserInput();
            UpdateTriggerTime();
            App.ClockControl.Timer.Tick += ClockTick;
        }



        //Methode

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveUserData(string nameOf)
        {
            Properties.Settings.Default.Save();
            OnPropertyChanged(nameOf);
        }

        private void ClockTick(object source, EventArgs e)
        {
            ClockTime = App.ClockControl.ClockTime.ToLongTimeString();
        }

        private void UpdateTriggerTime()
        {
            TriggerTime = new TimeSpan(ClockHours, ClockMinutes, ClockSeconds).ToString();
        }

        private void CheckEmptyUserInput()
        {
            if (ObserveTime == 0)
            {
                ObserveTime = 10;
            }

            if (Speed == 0)
            {
                Speed = 0.5;
            }

            if (!DownloadIsChecked && !UploadIsChecked)
            {
                DownloadIsChecked = true;
            }
            if(!ShutdownIsChecked &! RestartIsChecked &!SleepIsChecked)
            {
                ShutdownIsChecked = true;
            }
        }


        private int CheckMaxValue(int maxValue, int value)
        {
            if(value > maxValue)
            {
                return maxValue;
            }
            else
            {
                return value;
            }
        }


    }
}

