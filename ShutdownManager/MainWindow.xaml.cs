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

        ComputerFunctions pcFunktions = new ComputerFunctions();

        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            pcFunktions.timer.Tick += OnTimerTick; //Update Timer event
        }


        private void Button_Start(object sender, RoutedEventArgs e)
        {
            CheckEmptyUserInput();
            pcFunktions.StartTimer();
            
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            pcFunktions.StopTimer(true); //With BalloonTip
        }

        private void HoursTxt_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                int hours = Convert.ToInt32(txtHours.Text);
                if(hours > 23)
                {
                    pcFunktions.Hours = 23;
                    pcFunktions.Minutes = 59;
                    pcFunktions.Seconds = 59;
                }
                else
                {
                    pcFunktions.Hours = hours;
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
                if (pcFunktions.Hours > 22 && minutes > 59)
                {
                    pcFunktions.Minutes = 59;
                }
                else
                {
                    pcFunktions.Minutes = Convert.ToInt32(txtMinutes.Text);
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
                if (pcFunktions.Hours > 22 && pcFunktions.Minutes > 58 && seconds > 59)
                {
                    pcFunktions.Seconds = 59;
                }
                else
                {
                    pcFunktions.Seconds = Convert.ToInt32(txtSeconds.Text);
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
                TbTimer.Text = pcFunktions.TimeLeft.ToString(@"hh\:mm\:ss");
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
                pcFunktions.TimerZeroAction = eTimerZeroActions.Shutdown;
            }
            else if ((bool)RadioButton_Restart.IsChecked)
            {
                pcFunktions.TimerZeroAction = eTimerZeroActions.Restart;

            }
            else
            {
                pcFunktions.TimerZeroAction = eTimerZeroActions.EnergySafe;
            }
        }
    }
}
