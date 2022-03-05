using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShutdownManager.Commands;
using ShutdownManager.Utility;

namespace ShutdownManager.ViewModels
{

    public partial class MainWindowViewModel : INotifyPropertyChanged
    {


        //Variables

        private string _timeLeft;
        private TimeSpan _timeSpanLeft;
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

