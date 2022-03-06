using System;
using System.Net.NetworkInformation;
using System.Threading;
using ShutdownManager.Utility;
using Hardcodet.Wpf.TaskbarNotification;



namespace ShutdownManager.Classes
{
    public class DownUploadController
    {
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
            } 
        }


        //if view is activ, it would be start the Thread to read the up- and download
        public bool IsTapActiv
        {
            get => _isTapActive;
            set
            {
                _isTapActive = value;
                if (thUpdateValues != null)
                {
                    //start Thread
                    if (_isTapActive && !thUpdateValues.IsAlive)
                    {
                        thUpdateValues = new Thread(UpdateNetworkTraffic);
                        thUpdateValues.Start();
                    }
                    //stop Thread
                    else
                    {
                        AbortThreadWithConditions();
                    }
                }
                else
                {
                    //Create new Thread
                    thUpdateValues = new Thread(UpdateNetworkTraffic);
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
        private Thread thUpdateValues;
        private int _xSecondsUnderX = 0;

        public DownUploadController()
        {
        }


        //Abort thread if conditions OK
        private void AbortThreadWithConditions()
        {
            if (thUpdateValues.IsAlive && !_isObserveActive)
            {
                AbortThread();
            }
        }


        public void UpdateNetworkTraffic()
        {
            MyLogger.GetInstance().InfoWithClassName("Start Thread 'UpdateNetworkTraffic'", this);
            long maxReceived = 0;
            long maxSend = 0;
            long maxReceivedOld = 0;
            long maxSendOld = 0;
            double receivedMBS;
            double sendMBS;
            bool firstScan = true;


            while (true)
            {

                if (!NetworkInterface.GetIsNetworkAvailable())
                { 
                    App.ViewModel.DownloadValue = "";
                    App.ViewModel.UploadValue = "";
                    App.ViewModel.NoInternetConnection = true;
                    if (firstScan) { MyLogger.GetInstance().ErrorWithClassName("No internet connection", this);}
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
                        if (maxReceived < ni.GetIPv4Statistics().BytesReceived)
                        {
                            maxReceived = ni.GetIPv4Statistics().BytesReceived;
                        }
                        if (maxSend < ni.GetIPv4Statistics().BytesSent)
                        {
                            maxSend = ni.GetIPv4Statistics().BytesSent;
                        }
                    }


                    //If maxReceived or maxSend was reseted from the pc, Reset the other variables

                    if (maxReceived == 0 || maxSend == 0 && (maxReceivedOld != 0 || maxSendOld != 0))
                    {
                        maxReceivedOld = 0;
                        maxSendOld = 0;
                    }

                    if (firstScan)
                    {
                        MyLogger.GetInstance().InfoWithClassName("Starting read received and send data", this);
                        //First scan
                        maxReceivedOld = maxReceived;
                        maxSendOld = maxSend;

                    }
                    else
                    {
                        maxReceived -= maxReceivedOld;
                        maxSend -= maxSendOld;


                        receivedMBS = ((double)maxReceived / 1024.0) / 1024.0;   // (maxReceived / 1024) /1024 = MBit/s
                        sendMBS = ((double)maxSend / 1024.0) / 1024.0; // (maxSent / 1024) / 1024 = MBit / s

                        App.ViewModel.DownloadValue = Math.Round(receivedMBS, 1).ToString() + " MB/s";
                        App.ViewModel.UploadValue = Math.Round(sendMBS).ToString() + " MB/s";

                        maxReceivedOld += maxReceived;
                        maxSendOld += maxSend;


                        if (_isObserveActive)
                        {


                            /* if received or send MBs under the checked Speed add some#
                                else set it on zero */
                            if (ObserveFunction == LoadFunction.Download)
                            {
                                if (receivedMBS < App.ViewModel?.Speed)
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
                                if (sendMBS < App.ViewModel?.Speed)
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

                firstScan = false;
                Thread.Sleep(1000);
                
            }

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

        public void AbortThread()
        {
            MyLogger.GetInstance().InfoWithClassName("Abort Thread 'UpdateNetworkTraffic'", this);
            thUpdateValues?.Abort();
        }


        ~DownUploadController()
        {
            AbortThread();
        }


    }
}
