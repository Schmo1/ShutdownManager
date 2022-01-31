

namespace ShutdownManager.ViewModels
{
    public class SettingsViewModel 
    { 
        public bool AutoStartActiv { get => App.AppCon.AutoStart.AutoStartActiv; set => App.AppCon.AutoStart.ChangeAutoStartChecked(); }
        public bool WindowRunInTheBackground { get { return Properties.Settings.Default.OnWindowClosingActiv; } set { Properties.Settings.Default.OnWindowClosingActiv = value; Properties.Settings.Default.Save(); } }
        public bool OpenMinimized { get { return Properties.Settings.Default.OpenMinimized; } set { Properties.Settings.Default.OpenMinimized = value; Properties.Settings.Default.Save(); } }
        public bool DisablePushMessages { get { return Properties.Settings.Default.DisablePushMessages; } set { Properties.Settings.Default.DisablePushMessages = value; Properties.Settings.Default.Save(); } }
    }
}
