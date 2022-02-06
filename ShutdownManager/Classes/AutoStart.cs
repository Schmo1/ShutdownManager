﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using Forms = System.Windows.Forms;

namespace ShutdownManager.Classes
{



    public class AutoStart
    {

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private readonly string _appName;
        private string _args;

        public bool AutoStartActiv { get => IsAutoStartActiv(); }
        public string Arguments { get { return _args; } set { _args = value; EnableAutoStart(); } }

        

        public AutoStart()
        {
            _appName = Application.ResourceAssembly.GetName().Name;
        }
        public AutoStart(string args)
        {
           Arguments = args;
            _appName = Application.ResourceAssembly.GetName().Name;
        }

        public AutoStart(string args, string appName)
        {
            Arguments = args;
            _appName = appName;
        }

        private bool IsAutoStartActiv()
        {
            object objValue = startupKey.GetValue(_appName);

            // Check to see the current state (running at startup or not)
            if (objValue == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }
            else
            {
                // The value exists, the application is set to run at startup  
                try
                {
                    EnableAutoStart();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error GetValue", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }


        public void EnableAutoStart()
        {
            try
            {
                //Get Full Path
                string dirPath = GetRegPath();

                // Add the value in the registry so that the application runs at startup
                startupKey?.SetValue(_appName, dirPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error GetFullPath", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DisableAutoStart()
        {
            try
            {
                // Remove the value from the registry so that the application doesn't start
                startupKey?.DeleteValue(_appName, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error Delete Registry entry", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ChangeAutoStartChecked()
        {
            if (AutoStartActiv)
            {
                // Remove the value from the registry so that the application doesn't start
                DisableAutoStart();
            }
            else
            {
                EnableAutoStart();
            }
        }


        private string GetRegPath()
        {
            //Get Full Path
            string dirInfo = Path.GetFullPath(Forms.Application.ExecutablePath);
            //add quotation marks and argument
            dirInfo = $"\"{dirInfo}\"" + Arguments;

            return dirInfo;
        }
    }
}
