﻿using System.Windows;
using System.Windows.Input;
using ShutdownManager.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShutdownManager.ViewModels
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {

        public event PropertyChangedEventHandler PropertyChanged;



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


        // Hides the main window. This command is only enabled if a window is open.

        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => App.Window.Close(),
                    CanExecuteFunc = () => App.Window.IsVisible == true
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
        
    }


}
