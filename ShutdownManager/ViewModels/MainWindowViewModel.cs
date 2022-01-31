using System;
using System.Windows.Forms;
using System.ComponentModel;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.CompilerServices;

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



        //Properties
        public TimeSpan TimeSpanLeft { get { return _timeSpanLeft; } set { _timeSpanLeft = value; TimeLeft = _timeSpanLeft.ToString(@"hh\:mm\:ss"); } }
        public string TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(); } }
        public bool IsTimerStarted { get { return _isTimerStarted; } set { _isTimerStarted = value; OnPropertyChanged(); } }



        //Timer Functions
        public bool ShutdownIsChecked { get => Properties.Settings.Default.TimerShutdownIsChecked; set { Properties.Settings.Default.TimerShutdownIsChecked = value; Properties.Settings.Default.Save(); } }
        public bool RestartIsChecked { get => Properties.Settings.Default.TimerRestartIsChecked; set { Properties.Settings.Default.TimerRestartIsChecked = value;  Properties.Settings.Default.Save(); } }
        public bool SleepIsChecked { get => Properties.Settings.Default.TimerSleepIsChecked; set { Properties.Settings.Default.TimerSleepIsChecked = value; Properties.Settings.Default.Save(); } }


        // Down- Upload Functions
        public bool DownloadIsChecked { get => Properties.Settings.Default.DownloadIsChecked; set { Properties.Settings.Default.DownloadIsChecked = value; Properties.Settings.Default.Save(); } }

        public bool UploadIsChecked { get => Properties.Settings.Default.UploadIsChecked; set { Properties.Settings.Default.UploadIsChecked = value; Properties.Settings.Default.Save(); } }



        //Times for Timerfunctioncontrol
        public int Hours
        {
            get => Properties.Settings.Default.TimerHours;
            set
            {
                Properties.Settings.Default.TimerHours = value;
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get => Properties.Settings.Default.TimerMinutes;
            set
            {
                Properties.Settings.Default.TimerMinutes = value;
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged(); 
            }
        }

        public int Seconds
        {
            get => Properties.Settings.Default.TimerSeconds;
            set
            {
                Properties.Settings.Default.TimerSeconds = value;
                Properties.Settings.Default.Save();
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged();
            }
        }


        public bool IsObserveActiv { get { return App.DownUploadController.IsObserveActiv; } set { App.DownUploadController.IsObserveActiv = value; OnPropertyChanged(); }  }

        //Down- Upload Control
        public int ObserveTime
        {
            get => Properties.Settings.Default.ObserveTime;
            set
            {
                int maxValue = 99999;
                int minValue = 2;

                if (value > maxValue)
                {
                    Properties.Settings.Default.ObserveTime = maxValue;
                }
                else if (value < minValue)
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
                OnPropertyChanged();
            }
        }

        public string UploadValue
        {
            get => _uploadValue;
            set
            {
                _uploadValue = value;
                OnPropertyChanged();
            }
        }

        public bool DownUploadIsSelected
        {
            get => App.DownUploadController.IsTapActiv;
            set => App.DownUploadController.IsTapActiv = value;
        }



        //Events
        public event PropertyChangedEventHandler PropertyChanged;


        //Konstruktor
        public MainWindowViewModel()
        {
            OnPropertyChanged();
            CheckEmptyUserInput();
        }



        //Methode

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}

