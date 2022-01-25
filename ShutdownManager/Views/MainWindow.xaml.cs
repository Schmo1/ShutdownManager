using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ShutdownManager.Views
{

    public partial class MainWindow : Window
    {

        private const string downUploadText2 = "if the average XXXInsertDownUploadXXX speed is under XXXInsertSpeedXXX MB/s";
        private const string downUploadText3 = " for XXXInsertTimesXXX Times (s) then the PC will shut down";
        private const string speedInsertTemplate = "XXXInsertSpeedXXX";
        private const string timesInsertTemplate = "XXXInsertTimesXXX";
        private const string DownUploadInsertTemplate = "XXXInsertDownUploadXXX";

        public MainWindow()
        {
            
            
            InitializeComponent();

            DataContext = App.FunktionController;

            App.FunktionController.OnTimerIsOver += OnTimerisOver; //Timer is over event

        }


        private void Button_StartStop(object sender, RoutedEventArgs e)
        {
            if (!App.FunktionController.IsTimerStarted)
            {
                FillUpEmptyUserInput();
                ShowPauseImage();
                App.FunktionController.StartTimer();
            }
            else
            {
                ShowPlayImage();
                App.FunktionController.StopPauseTimer(true, true);
            }


        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ShowPlayImage();
            App.FunktionController.StopPauseTimer(false, true);
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
            if (App.FunktionController.IsTimerStarted)
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
            imageStop.Source = new BitmapImage(new Uri(@"/images/Stop.png", UriKind.Relative));

        }
        private void ShowPlayImage()
        {
            //Images
            imagePlayPause.Source = new BitmapImage(new Uri(@"/images/Play.png", UriKind.Relative));
            imageStop.Source = new BitmapImage(new Uri(@"/images/StopDeacti.png", UriKind.Relative));

        }

        private void UpdateDownUploadText()
        {
            if (tbTextDownUp2 != null && tbTextDownUp3 != null && tBSpeed != null && tBSeconds != null)
            {   //Replace Text
                tbTextDownUp2.Text = downUploadText2;
                tbTextDownUp3.Text = downUploadText3;

                tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(speedInsertTemplate, tBSpeed.Text);
                tbTextDownUp3.Text = tbTextDownUp3.Text.Replace(timesInsertTemplate, tBSeconds.Text);

                if (App.FunktionController.DownloadIsChecked)
                {
                    tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(DownUploadInsertTemplate, "Download");
                }
                else if (App.FunktionController.UploadIsChecked)
                {
                    tbTextDownUp2.Text = tbTextDownUp2.Text.Replace(DownUploadInsertTemplate, "Upload");
                }

            }

            RadioButton_Download.IsChecked = App.FunktionController.DownloadIsChecked ;
            RadioButton_Upload.IsChecked = App.FunktionController.UploadIsChecked;

        }

        private void RadioButton_DownUp_Click(object sender, RoutedEventArgs e)
        {

            App.FunktionController.DownloadIsChecked = (bool)RadioButton_Download.IsChecked;
            App.FunktionController.UploadIsChecked = (bool)RadioButton_Upload.IsChecked;
            UpdateDownUploadText();
        }


        private void TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateDownUploadText();
        }
    }
    
}
