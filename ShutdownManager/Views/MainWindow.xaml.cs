using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ShutdownManager.Views
{

    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.TimerFunktionController;

            App.TimerFunktionController.OnTimerIsOver += OnTimerisOver; //Timer is over event

        }



        private void Button_StartStop(object sender, RoutedEventArgs e)
        {
            if (!App.TimerFunktionController.IsTimerStarted)
            {
                FillUpEmptyUserInput();
                ShowPauseImage();
                App.TimerFunktionController.StartTimer();
            }
            else
            {
                ShowPlayImage();
                App.TimerFunktionController.StopPauseTimer(true);
            }


        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ShowPlayImage();
            App.TimerFunktionController.StopPauseTimer(false);
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
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
            if (App.TimerFunktionController.IsTimerStarted)
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

    }
    
}
