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

        public UserDataPersistentManager userDataPersistentManager = new UserDataPersistentManager(); //Class to manage the UserData's



        //Properties
        public TimeSpan TimeSpanLeft { get { return _timeSpanLeft; } set { _timeSpanLeft = value; TimeLeft = _timeSpanLeft.ToString(@"hh\:mm\:ss"); } }
        public string TimeLeft { get { return _timeLeft; } set { _timeLeft = value; OnPropertyChanged(); } }
        public bool IsTimerStarted { get { return _isTimerStarted; } set { _isTimerStarted = value; OnPropertyChanged(); } }



        //Timer Functions
        public bool ShutdownIsChecked { get => userDataPersistentManager.ShutdownIsChecked; set { userDataPersistentManager.ShutdownIsChecked = value; } }
        public bool RestartIsChecked { get => userDataPersistentManager.RestartIsChecked; set { userDataPersistentManager.RestartIsChecked = value; } }
        public bool SleepIsChecked { get => userDataPersistentManager.SleepIsChecked; set { userDataPersistentManager.SleepIsChecked = value; } }


        // Down- Upload Functions
        public bool DownloadIsChecked
        {
            get => userDataPersistentManager.DownloadIsChecked;
            set
            {
                userDataPersistentManager.DownloadIsChecked = value;
            }
        }


        public bool UploadIsChecked
        {
            get => userDataPersistentManager.UploadIsChecked;
            set
            {
                userDataPersistentManager.UploadIsChecked = value;
            }
        }



        //Times for Timerfunctioncontrol
        public int Hours
        {
            get => userDataPersistentManager.Hours;
            set
            {
                userDataPersistentManager.Hours = value;
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get => userDataPersistentManager.Minutes;
            set
            {
                userDataPersistentManager.Minutes = value;
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged(); 
            }
        }

        public int Seconds
        {
            get => userDataPersistentManager.Seconds;
            set
            {
                userDataPersistentManager.Seconds = value;
                App.TimerController.UpdateTimeSpan();
                OnPropertyChanged();
            }
        }


        public bool IsObserveActiv { get => App.DownUploadController.IsObserveActiv; set => App.DownUploadController.IsObserveActiv = value; }

        //Down- Upload Control
        public int ObserveTime
        {
            get => userDataPersistentManager.ObserveTime;
            set
            {
                int maxValue = 99999;
                int minValue = 2;

                if (value > maxValue)
                {
                    userDataPersistentManager.ObserveTime = maxValue;
                }
                else if (value < minValue)
                {
                    userDataPersistentManager.ObserveTime = minValue;
                }
                else
                {
                    userDataPersistentManager.ObserveTime = value;
                }
                App.DownUploadController.ObserveTime = userDataPersistentManager.ObserveTime;

            }
        }
        public double Speed
        {
            get => userDataPersistentManager.Speed;
            set
            {
                double maxValue = 1000;
                double minValue = 0.1;

                if (value > maxValue)
                {
                    userDataPersistentManager.Speed = maxValue;
                }
                else if (value < minValue)
                {
                    userDataPersistentManager.Speed = minValue;
                }
                else
                {
                    userDataPersistentManager.Speed = value;
                }
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
            userDataPersistentManager.LoadUserData();
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
        }
    }
}

