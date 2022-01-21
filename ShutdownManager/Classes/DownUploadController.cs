
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;



namespace ShutdownManager.Classes
{
    public class DownUploadController
    {
        public enum LoadFunction { Download, Upload }

        public LoadFunction ObserveFunction { 
            
            get  {
                if (App.FunktionController.userDataPersistentManager.UploadIsChecked) { return LoadFunction.Upload; }
                else { return LoadFunction.Download; }
            }
        }

        public bool IsObserveActiv
        {
            get { return _isObserveActive; }
            
            set 
            {
                CreateTaskbarMessage(value);
                if (value)
                {
                    _expiredObserveTime = 0;
                }
                _isObserveActive = value;
            } 
        }
        public int ObserveTime
        {
            get { return _observeTime; }
            set { _observeTime = value; CreateArrNew(); }
        }

        public bool IsViewActiv
        {
            get => _isViewActive;
            set
            {
                _isViewActive = value;
                if (thUpdateValues != null)
                {
                    //start Thread
                    if (_isViewActive && !thUpdateValues.IsAlive)
                    {
                        thUpdateValues = new Thread(UpdateNetworkTraffic);
                        thUpdateValues.Start();
                    }
                    //start Thread
                    else if (thUpdateValues.IsAlive && !_isObserveActive)
                    {
                        try
                        {
                            thUpdateValues.Abort();
                        }
                        catch (Exception)
                        {

                        }
                        
                    }
                }
                else
                {
                    thUpdateValues = new Thread(UpdateNetworkTraffic);
                }
            }
            
        }

        private int _observeTime;
        private bool _isObserveActive;
        private bool _isViewActive;
        private NetworkInterface[] interfaces;
        private Thread thUpdateValues;
        private double[] recordedSpeeds;
        private int _expiredObserveTime;

        public DownUploadController()
        {
        }


        public void UpdateNetworkTraffic()
        {

            long maxReceived = 0;
            long maxSend = 0;
            long maxReceivedOld = 0;
            long maxSendOld = 0;
            double receivedMBS;
            double sendMBS;
            bool firstScan = true;

            CreateArrNew();


            while (true)
            {

                if (!NetworkInterface.GetIsNetworkAvailable())
                    return;

                try
                {
                    interfaces = NetworkInterface.GetAllNetworkInterfaces();
                }catch (ThreadAbortException)
                {
                    //nothing                    
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message.ToString(), "DownUploadController", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


                if (firstScan)
                {
                    //First scan
                    maxReceivedOld = maxReceived;
                    maxSendOld = maxSend;
                    firstScan = false;  
                }
                else
                {
                    maxReceived -= maxReceivedOld;
                    maxSend -= maxSendOld;

                    
                    receivedMBS = ((double)maxReceived / 1024.0) / 1024.0;   // (maxReceived / 1024) /1024 = MBit/s
                    sendMBS = ((double)maxSend / 1024.0) / 1024.0; // (maxSent / 1024) / 1024 = MBit / s

                    App.FunktionController.DownloadValue = Math.Round(receivedMBS,1).ToString() + " MB/s"; 
                    App.FunktionController.UploadValue = Math.Round(sendMBS).ToString() + " MB/s"; 

                    Console.WriteLine("Download: {0}", App.FunktionController.DownloadValue);
                    Console.WriteLine("Upload: {0}", App.FunktionController.UploadValue);
                    Console.WriteLine();

                    maxReceivedOld += maxReceived;
                    maxSendOld += maxSend;

                    
                    if (_isObserveActive && recordedSpeeds.Length != 0)
                    {
                        _expiredObserveTime++;

                        //Move the Array
                        for (int i = recordedSpeeds.Length - 1; i > 0; i--)
                        {
                            recordedSpeeds[i] = recordedSpeeds[i - 1];
                        }

                        if (ObserveFunction == LoadFunction.Download)
                        {
                            recordedSpeeds[0] = receivedMBS;
                        }
                        else
                        {
                            recordedSpeeds[0] = sendMBS;
                        }


                        //Check if Average of the Speed is under the Observe speed and on the first time they should do nothing
                        if (IsAverageUnderSpeed() && (_expiredObserveTime > App.FunktionController.ObserveTime))
                        {
                            ObserveIsOver();
                        }

                    }
                }
               
                Thread.Sleep(1000);
            }

        }

        private void CreateArrNew()
        {
            recordedSpeeds = new double[App.FunktionController.ObserveTime];
        }

        private bool IsAverageUnderSpeed()
        {
            if(GetAverage() < App.FunktionController.Speed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private double GetAverage()
        {
            double average = 0;
            
            foreach (double value in recordedSpeeds)
            {
                average += value;
            }
            if(average != 0)
            {
                average /= (recordedSpeeds.Length + 1);
            }
            
            return average;
        }


        private void CreateTaskbarMessage(bool stateActivated)
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

            App.TaskbarIcon.ShowBalloonTip("Info", message, BalloonIcon.Info);
        }

        private void ObserveIsOver()
        {
            _expiredObserveTime = 0;
            //App.ShutdownOptions.Shutdown();
            MessageBox.Show(" Average speed");
        }

        public void AbortThread()
        {
            thUpdateValues.Abort();
        }


        ~DownUploadController()
        {
            thUpdateValues.Abort();
        }


    }
}
