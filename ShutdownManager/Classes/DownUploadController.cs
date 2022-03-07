using System;
using Timers = System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using ShutdownManager.Utility;
using Hardcodet.Wpf.TaskbarNotification;



namespace ShutdownManager.Classes
{
    public class DownUploadController
    {
        private Timers.Timer _timer;
        long _maxReceived = 0;
        long _maxSend = 0;
        long _maxReceivedOld = 0;
        long _maxSendOld = 0;
        double _receivedMBS;
        double _sendMBS;
        bool _firstScan = true;

        public enum LoadFunction { Download, Upload }

        public LoadFunction ObserveFunction { 
            
            get  {
                if (App.ViewModel.UploadIsChecked) { return LoadFunction.Upload; }
                else { return LoadFunction.Download; }
            }
        }

        public bool IsObserveActiv
        {
            get { return _isObserveActive; }
            
            set 
            {
                CreateBalloonTip(value);
                XSecondsUnderX = 0;
                _isObserveActive = value;
                //Set value only when it 
                if (_isObserveActive)
                    App.NotifyIconViewModel.SystemTrayMenuText = "Down- Upload observing is activ";
                else
                    App.NotifyIconViewModel.SetSystemTrayMenuTextToDefault();
                
            } 
        }


        //if view is activ, it would be start the Thread to read the up- and download
        public bool IsTapActiv
        {
            get => _isTapActive;
            set
            {
                _isTapActive = value;
                
                //start Timer
                if (_isTapActive)
                {
                    _timer?.Start();
                    _firstScan = true;
                }
                //stop Timer
                else
                {                    
                    _timer?.Stop();
                }
            } 
        }


        private int XSecondsUnderX 
        { 
            get { return _xSecondsUnderX; } 
            set {
                    _xSecondsUnderX = value;
                    if(App.ViewModel != null)
                        App.ViewModel.PrBaValue = value;
                } 
        }


        private bool _isObserveActive;
        private bool _isTapActive;
        private NetworkInterface[] interfaces;
        private int _xSecondsUnderX = 0;

        public DownUploadController()
        {
            _timer = new Timers.Timer{Interval = 1000};
            _timer.Tick += new EventHandler(TimerEventTick);
        }

        private void TimerEventTick(Object myObject, EventArgs myEventArgs)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                App.ViewModel.DownloadValue = "";
                App.ViewModel.UploadValue = "";
                App.ViewModel.NoInternetConnection = true;
                if (_firstScan) { MyLogger.GetInstance().ErrorWithClassName("No internet connection", this); }
            }
            else
            {

                App.ViewModel.NoInternetConnection = false;

                try
                {
                    interfaces = NetworkInterface.GetAllNetworkInterfaces();
                }
                catch (ThreadAbortException)
                {
                    //nothing                    
                }
                catch (Exception e)
                {

                    MyLogger.GetInstance().ErrorWithClassName("GetAllNetworkInterfaces. Exception " + e.Message, this);
                }

                foreach (NetworkInterface ni in interfaces)
                {
                    if (_maxReceived < ni.GetIPv4Statistics().BytesReceived)
                    {
                        _maxReceived = ni.GetIPv4Statistics().BytesReceived;
                    }
                    if (_maxSend < ni.GetIPv4Statistics().BytesSent)
                    {
                        _maxSend = ni.GetIPv4Statistics().BytesSent;
                    }
                }


                //If maxReceived or maxSend was reseted from the pc, Reset the other variables

                if (_maxReceived == 0 || _maxSend == 0 && (_maxReceivedOld != 0 || _maxSendOld != 0))
                {
                    _maxReceivedOld = 0;
                    _maxSendOld = 0;
                }

                if (_firstScan)
                {
                    MyLogger.GetInstance().InfoWithClassName("Starting read received and send data", this);
                    //First scan
                    _maxReceivedOld = _maxReceived;
                    _maxSendOld = _maxSend;

                }
                else
                {
                    _maxReceived -= _maxReceivedOld;
                    _maxSend -= _maxSendOld;


                    _receivedMBS = ((double)_maxReceived / 1024.0) / 1024.0;   // (maxReceived / 1024) /1024 = MBit/s
                    _sendMBS = ((double)_maxSend / 1024.0) / 1024.0; // (maxSent / 1024) / 1024 = MBit / s

                    App.ViewModel.DownloadValue = Math.Round(_receivedMBS, 1).ToString() + " MB/s";
                    App.ViewModel.UploadValue = Math.Round(_sendMBS).ToString() + " MB/s";

                    _maxReceivedOld += _maxReceived;
                    _maxSendOld += _maxSend;


                    if (_isObserveActive)
                    {


                        /* if received or send MBs under the checked Speed add some#
                            else set it on zero */
                        if (ObserveFunction == LoadFunction.Download)
                        {
                            if (_receivedMBS < App.ViewModel?.Speed)
                            {
                                XSecondsUnderX++;
                            }
                            else
                            {
                                XSecondsUnderX = 0;
                            }
                        }
                        else // upload
                        {
                            if (_sendMBS < App.ViewModel?.Speed)
                            {
                                XSecondsUnderX++;
                            }
                            else
                            {
                                XSecondsUnderX = 0;
                            }
                        }

                        if (App.ViewModel?.ObserveTime < XSecondsUnderX)
                        {//Time is over
                            ObserveIsOver();
                        }
                    }
                }
            }

            _firstScan = false;

        }






        private void CreateBalloonTip(bool stateActivated)
        {

            string message = "Observing the Down/Upload is activ/inactiv!";

            if (ObserveFunction == LoadFunction.Download)
            {
                message = message.Replace("Down/Upload", "Download");
            }
            else
            {
                message = message.Replace("Down/Upload", "Upload");
            }

            if (stateActivated)
            {
                message = message.Replace("activ/inactiv", "activ");
            }
            else
            {
                message = message.Replace("activ/inactiv", "inactiv");
            }

            App.ShowBalloonTip("Info", message, BalloonIcon.Info);
        }

        private void ObserveIsOver()
        {
            MyLogger.GetInstance().InfoWithClassName("Observing Down- Upload is over", this);
            XSecondsUnderX = 0;

            if(App.ViewModel.RestartIsCheckedDownUP)
                ShutdownOptions.Instance.Restart();
            else if (App.ViewModel.ShutdownIsCheckedDownUP)
                ShutdownOptions.Instance.Shutdown();
            else
                ShutdownOptions.Instance.Sleep();
        }

    }
}
