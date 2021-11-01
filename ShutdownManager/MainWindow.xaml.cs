using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ShutdownManager
{

    

    public partial class MainWindow : Window
    {

        TimerFunktionController timerController = new TimerFunktionController();

        //Constanten
        private const string balloonTipTitle = "ShutdownManager";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            timerController.timer.Tick += OnTimerTick; //Update Timer event
        }


        private void Button_Start(object sender, RoutedEventArgs e)
        {
            CheckEmptyUserInput();
            if (timerController.TimerHasStarted)
            {
                string message = "Timer has started";
                MyNotifyIcon.ShowBalloonTip(balloonTipTitle, message, BalloonIcon.Info);
            }

            timerController.StartTimer();
            
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            string message = "Timer has stopped";
            MyNotifyIcon.ShowBalloonTip(balloonTipTitle, message, BalloonIcon.Info);

            timerController.StopTimer(); 
        }

        private void HoursTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                int hours = Convert.ToInt32(txtHours.Text);
                if(hours > 23)
                {
                    timerController.Hours = 23;
                    timerController.Minutes = 59;
                    timerController.Seconds = 59;
                }
                else
                {
                    timerController.Hours = hours;
                }

                UpdateTimer();
            }
            catch (FormatException ex)
            {
                CheckFormatException(ex, txtHours.Text);

            }


        }

        private void MinutesTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                int minutes = Convert.ToInt32(txtMinutes.Text);
                if (timerController.Hours > 22 && minutes > 59)
                {
                    timerController.Minutes = 59;
                }
                else
                {
                    timerController.Minutes = Convert.ToInt32(txtMinutes.Text);
                }

                    
                UpdateTimer();
            }
            catch (FormatException ex)
            {
                CheckFormatException(ex, txtMinutes.Text);

            }
            


        }
        private void SecondsTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                int seconds = Convert.ToInt32(txtSeconds.Text);
                if (timerController.Hours > 22 && timerController.Minutes > 58 && seconds > 59)
                {
                    timerController.Seconds = 59;
                }
                else
                {
                    timerController.Seconds = Convert.ToInt32(txtSeconds.Text);
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
                TbTimer.Text = timerController.TimeLeft.ToString(@"hh\:mm\:ss");
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Timespan", "Invalid Format", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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


        private void OnTimerTick(object source, EventArgs args)
        {
            UpdateTimer();
        }

        private void RadioButton_Update(object sender, RoutedEventArgs e)
        {
            if ((bool)RadioButton_Shutdown.IsChecked)
            {
                timerController.TimerZeroAction = eTimerZeroActions.Shutdown;
            }
            else if ((bool)RadioButton_Restart.IsChecked)
            {
                timerController.TimerZeroAction = eTimerZeroActions.Restart;

            }
            else
            {
                timerController.TimerZeroAction = eTimerZeroActions.EnergySafe;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
        }
    }
}
