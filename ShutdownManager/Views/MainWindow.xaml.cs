using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ShutdownManager.Views
{

    public partial class MainWindow : Window
    {

        private string downUploadText2 = App.AppCon.RManager.GetString("mainUpDownLine2");
        private  string downUploadText3 = App.AppCon.RManager.GetString("mainUpDownLine3");
        private const string speedInsertTemplate = "XXXInsertSpeedXXX";
        private const string timesInsertTemplate = "XXXInsertTimesXXX";
        private const string DownUploadInsertTemplate = "XXXInsertDownUploadXXX";
        private const string DownUploadActionInsertTemplate = "XXXInsertActionXXX";

        public MainWindow()
        { 
            InitializeComponent();

            DataContext = App.ViewModel;
            

            App.TimerController.OnTimerIsOver += OnTimerisOver; //Timer is over event


        }


        private void Button_StartPause(object sender, RoutedEventArgs e)
        {
            if (!App.TimerController.IsTimerStarted)
            {
                FillUpEmptyUserInput();
                ShowPauseImage();
                App.TimerController.StartTimer();
            }
            else
            {
                ShowPlayImage();
                App.TimerController.StopPauseTimer(true, true);
            }


        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ShowPlayImage();
            App.TimerController.StopPauseTimer(false, true);
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberValidationTextBoxDouble(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[0 - 9] + (\.[0 - 9]+)?");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void FillUpEmptyUserInput()
        {
            if (txtHours.Text.Length == 0) { txtHours.Text = "0"; }
            if (txtMinutes.Text.Length == 0) { txtMinutes.Text = "0"; }
            if (txtSeconds.Text.Length == 0) { txtSeconds.Text = "0"; }

        }

        public void LoadInformations()
        {
            if (App.TimerController.IsTimerStarted)
            {
                ShowPauseImage();
            }
            else
            {
                ShowPlayImage();
            }
        }


        private void OnTimerisOver(object source, EventArgs args)
        {
            ShowPlayImage();
        }



        private void ShowPauseImage()
        {
            //Images
            imagePlayPause.Source = new BitmapImage(new Uri(@"/images/Pause.png", UriKind.Relative));

        }
        private void ShowPlayImage()
        {
            //Images
            imagePlayPause.Source = new BitmapImage(new Uri(@"/images/Play.png", UriKind.Relative));

        }

        private void UpdateDownUploadText()
        {
            if (tbTextDownUp2 != null && tbTextDownUp3 != null && tBSpeed != null && tBSeconds != null)
            {   //Replace Text
                tbTextDownUp2.Text = downUploadText2;
                tbTextDownUp3.Text = downUploadText3;

                tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(speedInsertTemplate, tBSpeed.Text);
                tbTextDownUp3.Text = tbTextDownUp3.Text.Replace(timesInsertTemplate, tBSeconds.Text);

                if (App.ViewModel.DownloadIsChecked)
                {
                    tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(DownUploadInsertTemplate, "Download");
                }
                else if (App.ViewModel.UploadIsChecked)
                {
                    tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(DownUploadInsertTemplate, "Upload");
                }

                if (App.ViewModel.ShutdownIsCheckedDownUP)
                {
                    tbTextDownUp3.Text = tbTextDownUp3.Text.Replace(DownUploadActionInsertTemplate, App.AppCon.RManager.GetString("shutdown").ToLower());
                }else if (App.ViewModel.RestartIsCheckedDownUP)
                {
                    tbTextDownUp3.Text = tbTextDownUp3.Text.Replace(DownUploadActionInsertTemplate, App.AppCon.RManager.GetString("restart").ToLower());
                }else
                {
                    tbTextDownUp3.Text = tbTextDownUp3.Text.Replace(DownUploadActionInsertTemplate, App.AppCon.RManager.GetString("sleep").ToLower());
                }

            }

            RadioButton_Download.IsChecked = App.ViewModel.DownloadIsChecked ;
            RadioButton_Upload.IsChecked = App.ViewModel.UploadIsChecked;

        }

        private void RadioButton_DownUp_Click(object sender, RoutedEventArgs e)
        {

            App.ViewModel.DownloadIsChecked = (bool)RadioButton_Download.IsChecked;
            App.ViewModel.UploadIsChecked = (bool)RadioButton_Upload.IsChecked;
            UpdateDownUploadText();
        }


        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateDownUploadText();
        }

        private void Slider_ClockHours_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.ClockHours = App.ClockControl.ClockTime.Hour;
        }
        private void Slider_ClockMinutes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.ClockMinutes = App.ClockControl.ClockTime.Minute;
        }
        private void Slider_ClockSeconds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.ClockSeconds = App.ClockControl.ClockTime.Second;
        }



        //Radiobutton without binding
        private void rb_CheckedClock(object sender, RoutedEventArgs e)
        {
            App.ViewModel.ShutdownClockIsChecked = (bool)rbShutdownClock.IsChecked;
            App.ViewModel.RestartClockIsChecked = (bool)rbRestartClock.IsChecked;
            App.ViewModel.SleepClockIsChecked = (bool)rbSleepClock.IsChecked;
            
        }

        private void rb_CheckedDownUp(object sender, RoutedEventArgs e)
        {
            App.ViewModel.ShutdownIsCheckedDownUP = (bool)rbShutdownActionUpDown.IsChecked;
            App.ViewModel.RestartIsCheckedDownUP = (bool)rbRestartActionUpDown.IsChecked;
            App.ViewModel.SleepIsCheckedDownUP = (bool)rbSleepActionUpDown.IsChecked;
            UpdateDownUploadText();
        }


        private void TIClock_GotFocus(object sender, RoutedEventArgs e)
        {
            rbShutdownClock.IsChecked = App.ViewModel.ShutdownClockIsChecked;
            rbRestartClock.IsChecked = App.ViewModel.RestartClockIsChecked;
            rbSleepClock.IsChecked = App.ViewModel.SleepClockIsChecked;

        }

        private void TIDownUpload_GotFocus(object sender, RoutedEventArgs e)
        {
            rbShutdownActionUpDown.IsChecked = App.ViewModel.ShutdownIsCheckedDownUP;
            rbRestartActionUpDown.IsChecked = App.ViewModel.RestartIsCheckedDownUP;
            rbSleepActionUpDown.IsChecked = App.ViewModel.SleepIsCheckedDownUP;
        }
    }
    
}
