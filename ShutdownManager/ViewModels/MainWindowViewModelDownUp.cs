using System;

namespace ShutdownManager.ViewModels
{
    public partial class MainWindowViewModel
    {

        //Variables
        private string _uploadValue;
        private string _downloadValue;
        private int _prBaValue;
        private bool _hasInternetConnection;


        //DownUpload Functions
        public bool ShutdownIsCheckedDownUP { get => Properties.Settings.Default.ShutdownIsCheckedDownUP; set { Properties.Settings.Default.ShutdownIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }
        public bool RestartIsCheckedDownUP { get => Properties.Settings.Default.RestartIsCheckedDownUP; set { Properties.Settings.Default.RestartIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }
        public bool SleepIsCheckedDownUP { get => Properties.Settings.Default.SleepIsCheckedDownUP; set { Properties.Settings.Default.SleepIsCheckedDownUP = value; Properties.Settings.Default.Save(); } }


        public bool DownloadIsChecked { get => Properties.Settings.Default.DownloadIsChecked; set { Properties.Settings.Default.DownloadIsChecked = value; Properties.Settings.Default.Save(); } }

        public bool UploadIsChecked { get => Properties.Settings.Default.UploadIsChecked; set { Properties.Settings.Default.UploadIsChecked = value; Properties.Settings.Default.Save(); } }


        public bool IsObserveDownUploadActiv
        { 
            get { return App.DownUploadController.IsObserveActiv; } 
            set 
            {  
                App.DownUploadController.IsObserveActiv = value;
                OnPropertyChanged(nameof(IsObserveDownUploadActiv)); 
            } 
        }



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
                SaveUserData(nameof(ObserveTime));

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

        public int PrBaValue
        { 
            get => _prBaValue;
            set { _prBaValue = value;
                OnPropertyChanged(nameof(PrBaValue));
            } 
        }
        public bool NoInternetConnection 
        {
                get => _hasInternetConnection;
                set
            {
                    _hasInternetConnection = value;
                    OnPropertyChanged(nameof(NoInternetConnection));
                
            }
        }
    }
}
