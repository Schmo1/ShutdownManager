﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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

        public NotifyIconViewModel()
        {
            SetSystemTrayMenuTextToDefault();
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


        public string SystemTrayMenuText { get { return _sysTrayMenuText; } set { _sysTrayMenuText = value; OnPropertyChanged(); } }




        public void SetSystemTrayMenuTextToDefault()
        {
            SystemTrayMenuText = Application.ResourceAssembly.GetName().Name;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



}
