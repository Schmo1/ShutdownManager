using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using ShutdownManager.Classes;

namespace ShutdownManager.Installer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        

        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

          
        }
        public override void Uninstall(IDictionary savedState)
        {

            base.Uninstall(savedState);

        }
    }
}
