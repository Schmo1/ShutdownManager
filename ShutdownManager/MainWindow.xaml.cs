using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShutdownManager
{

    public partial class MainWindow : Window
    {
        private readonly TimerFunktionController timerFunktionController = new TimerFunktionController();
        

        //Constanten
        private const string balloonTipTitle = "ShutdownManager";

        //Images
        private readonly ImageSource imagePlayPNG;
        private readonly ImageSource imagePausePNG;
        private readonly ImageSource imageStopPNG;
        private readonly ImageSource imageStopDeactiPNG;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            timerFunktionController.timer.Tick += OnTimerTick; //Update Timer event
            timerFunktionController.OnTimerIsOver += OnTimerisOver;

            imagePlayPNG = new BitmapImage(new Uri(@"/images/Play.png", UriKind.Relative));
            imagePausePNG = new BitmapImage(new Uri(@"/images/Pause.png", UriKind.Relative));
            imageStopPNG = new BitmapImage(new Uri(@"/images/Stop.png", UriKind.Relative));
            imageStopDeactiPNG = new BitmapImage(new Uri(@"/images/StopDeacti.png", UriKind.Relative));

            DeactivateStopButton();

            timerFunktionController.userDataPersistentManager.LoadUserData();
            UpdateLoadedUserData();
            timerFunktionController.UpdateTimeSpan();
        }


        private void Button_StartStop(object sender, RoutedEventArgs e)
        {
            CheckEmptyUserInput();
            if (!timerFunktionController.TimerHasStarted)
            {
                MyNotifyIcon.ShowBalloonTip(balloonTipTitle, "Timer has started", BalloonIcon.Info); //Ballon Tip for user
                DeactivateStartButton();
                timerFunktionController.StartTimer();
            }
            else
            {
                MyNotifyIcon.ShowBalloonTip(balloonTipTitle, "Timer has paused", BalloonIcon.Info); //Ballon Tip for user
                DeactivateStopButton();
                timerFunktionController.StopPauseTimer(true);
            }


        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            MyNotifyIcon.ShowBalloonTip(balloonTipTitle, "Timer has stopped", BalloonIcon.Info); //Ballon Tip for user

            DeactivateStopButton();
            timerFunktionController.StopPauseTimer(false);
            UpdateTimer();
        }


        private void HoursTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            ChangeHours();
        }

        private void MinutesTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            ChangeMinutes();
        }
        private void SecondsTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            ChangeSeconds();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CheckFormatException (FormatException ex, string text)
        {
            if (text != null && text != "")
            {
                MessageBox.Show(ex.ToString(), "Invalid Format", MessageBoxButton.OK);
            }
        }

        private void CheckEmptyUserInput()
        {
            if(txtHours.Text.Length == 0)
            {
                txtHours.Text = "00";
            }

            if (txtMinutes.Text.Length == 0)
            {
                txtMinutes.Text = "00";
            }

            if (txtSeconds.Text.Length == 0)
            {
                txtSeconds.Text = "00";
            }
        }

        private void UpdateLoadedUserData()
        {
            txtHours.Text = timerFunktionController.Hours.ToString();
            txtMinutes.Text = timerFunktionController.Minutes.ToString();
            txtSeconds.Text = timerFunktionController.Seconds.ToString();

            switch (timerFunktionController.TimerAction)
            {
                case ETimerActions.Shutdown:
                    RadioButton_Shutdown.IsChecked = true;
                    RadioButton_Restart.IsChecked = false;
                    RadioButton_Sleep.IsChecked = false;
                    break;

                case ETimerActions.Restart:
                    RadioButton_Shutdown.IsChecked = false;
                    RadioButton_Restart.IsChecked = true;
                    RadioButton_Sleep.IsChecked = false;
                    break;

                default:
                    RadioButton_Shutdown.IsChecked = false;
                    RadioButton_Restart.IsChecked = false;
                    RadioButton_Sleep.IsChecked = true;
                    break;
            }
        }


        private void OnTimerTick(object source, EventArgs args)
        {
            UpdateTimer();
        }
        private void OnTimerisOver(object source, EventArgs args)
        {
            DeactivateStopButton();
            UpdateTimer();
        }

        private void RadioButton_Update(object sender, RoutedEventArgs e)
        {
            if ((bool)RadioButton_Shutdown.IsChecked)
            {
                timerFunktionController.TimerAction = ETimerActions.Shutdown;
            }
            else if ((bool)RadioButton_Restart.IsChecked)
            {
                timerFunktionController.TimerAction = ETimerActions.Restart;
            }
            else
            {
                timerFunktionController.TimerAction = ETimerActions.Sleep;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
        }

        private void DeactivateStartButton()
        {
            //Buttons
            btStop.IsEnabled = true;

            //Images
            imagePlayPause.Source = imagePausePNG;
            imageStop.Source = imageStopPNG;

            //Textfields
            txtHours.IsEnabled = false;
            txtMinutes.IsEnabled = false;
            txtSeconds.IsEnabled = false;
        }
        private void DeactivateStopButton()
        {
            //Buttons
            btStop.IsEnabled = false;


            //Images
            imagePlayPause.Source = imagePlayPNG;
            imageStop.Source = imageStopDeactiPNG;

            //Textfields
            txtHours.IsEnabled = true;
            txtMinutes.IsEnabled = true;
            txtSeconds.IsEnabled = true;
        }

        private void ChangeHours()
        {
            try
            {
                int hours = Convert.ToInt32(txtHours.Text);
                if (hours > 23)
                {
                    //if the hours ar over 24, it try to count the days. But there are no days in the Programm. 
                    timerFunktionController.Hours = 23;
                    timerFunktionController.Minutes = 59;
                    timerFunktionController.Seconds = 59;
                }
                else
                {
                    timerFunktionController.Hours = hours;
                }

                UpdateTimer();
            }
            catch (FormatException ex)
            {
                CheckFormatException(ex, txtHours.Text);
            }
        }

        private void ChangeMinutes()
        {
            try
            {
                int minutes = Convert.ToInt32(txtMinutes.Text);
                if (timerFunktionController.Hours > 22 && minutes > 59)
                {
                    timerFunktionController.Minutes = 59;
                }
                else
                {
                    timerFunktionController.Minutes = Convert.ToInt32(txtMinutes.Text);
                }


                UpdateTimer();
            }
            catch (FormatException ex)
            {
                CheckFormatException(ex, txtMinutes.Text);

            }

        }

        private void ChangeSeconds()
        {
            try
            {
                int seconds = Convert.ToInt32(txtSeconds.Text);
                if (timerFunktionController.Hours > 22 && timerFunktionController.Minutes > 58 && seconds > 59)
                {
                    timerFunktionController.Seconds = 59;
                }
                else
                {
                    timerFunktionController.Seconds = Convert.ToInt32(txtSeconds.Text);
                }

                UpdateTimer();
            }
            catch (FormatException ex)
            {
                CheckFormatException(ex, txtSeconds.Text);
            }
        }

        private void UpdateTimer()
        {
            try
            {
                TbTimer.Text = timerFunktionController.TimeLeft.ToString(@"hh\:mm\:ss");
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Timespan", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


    }
    
}
