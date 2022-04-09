using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ShutdownManager.Commands;

namespace ShutdownManager.ViewModels
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel : INotifyPropertyChanged
    {

        private string _sysTrayMenuText;
        private ImageSource _showIcon;
        private ImageSource _hideIcon;


        public NotifyIconViewModel()
        {
            _sysTrayMenuText = App.AppCon.AppName;
        }

        // Shows a window, if none is already open.

        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => App.Window.IsVisible == false,

                    CommandAction = () =>
                    {
                        App.OpenMainWindow();
                    }
                };
            }
        }
        public ICommand ShowWindowDoubleClickCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,

                    CommandAction = () =>
                    {
                        App.OpenMainWindow();
                    }
                };
            }
        }

        public ICommand OpenSettings
        {
            get
            {
                return new DelegateCommand { CommandAction = () => App.OpenSettings() };
            }
        }


        // Hides the main window. This command is only enabled if a window is open.

        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {

                    CommandAction = () => { App.HideWindowPressed = true; 
                                            App.Window.Close();  },
                    CanExecuteFunc = () => App.Window.IsVisible == true,
                };
            }
        }


        // Shuts down the application.

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }


        public string SystemTrayMenuText 
        { 
            get { return _sysTrayMenuText; } 
            set
            {//write only, when _sysTrayMenuText is AppName or _sysTrayMenuText is AppName
                if (value == App.AppCon.AppName || _sysTrayMenuText == App.AppCon.AppName)
                {
                    _sysTrayMenuText = value;
                    OnPropertyChanged();
                }

            } 
        }


        public ImageSource ShowIcon { get => _showIcon; private set { _showIcon = value; OnPropertyChanged(); } }
        public ImageSource HideIcon { get => _hideIcon; private set { _hideIcon = value; OnPropertyChanged(); } }


        public void SetSystemTrayMenuTextToDefault()
        {
            SystemTrayMenuText = App.AppCon.AppName;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateShowAndHideIcon()
        {
            if (App.Window.IsVisible)
            {
                ShowIcon = new BitmapImage(new Uri(@"/icons/ShowDisabled.ico", UriKind.Relative));
                HideIcon = new BitmapImage(new Uri(@"/icons/Hide.ico", UriKind.Relative));
            }
            else
            {
                ShowIcon = new BitmapImage(new Uri(@"/icons/Show.ico", UriKind.Relative));
                HideIcon = new BitmapImage(new Uri(@"/icons/HideDisabled.ico", UriKind.Relative));
            }

}
    }



}
