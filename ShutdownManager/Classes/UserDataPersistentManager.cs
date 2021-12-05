using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using ShutdownManager;

namespace ShutdownManager.Classes
{
    
    public class UserDataPersistentManager : UserData
    {
        
        private string _path;
        private bool _isLoaded = false;
        

        public UserDataPersistentManager()
        {
            try
            {
                //create Path for AppData
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = Path.Combine(folder, App.AppCon.AppName);

                // CreateDirectory will check if every folder in path exists and, if not, create them.
                // If all folders exist then CreateDirectory will do nothing.
                Directory.CreateDirectory(folder);

                const string jsonFileName = @"\UserData.json";
                _path = folder + jsonFileName;
            }
            catch (UnauthorizedAccessException)
            {
                string message = _path.Remove(0, 1);
                MessageBox.Show(($"No write access to the user data`s ({message})"), "UserDataPersistentManager.SaveData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message.ToString(), "UserDataPersistentManager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        public void SaveUserData()
        {
            if (_isLoaded) //UserData should first load, because at the beginnint the Values ar 0
            {
                try
                {
                    StreamWriter sw = new StreamWriter(_path, false);
                    UserData saveData = new UserData(Hours, Minutes, Seconds, ShutdownIsChecked, RestartIsChecked, SleepIsChecked);
                    string userDataStr = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                    sw.Write(userDataStr);
                    sw.Close();
                }catch (UnauthorizedAccessException)
                {
                    string message = _path.Remove(0, 1);
                    MessageBox.Show(($"No write access to the user data`s ({message})"), "UserDataPersistentManager.SaveData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message.ToString(), "UserDataPersistentManager.SaveData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        public void LoadUserData()
        {

            if (File.Exists(_path))
            {
                try
                {
                    StreamReader sr = new StreamReader(_path);

                    UserData loadData = new UserData();
                    string userDataStr = sr.ReadToEnd();

                    loadData = JsonConvert.DeserializeObject<UserData>(userDataStr);
                    sr.Close();

                    Hours = loadData.Hours;
                    Minutes = loadData.Minutes;
                    Seconds = loadData.Seconds;
                    ShutdownIsChecked = loadData.ShutdownIsChecked;
                    RestartIsChecked = loadData.RestartIsChecked;
                    SleepIsChecked = loadData.SleepIsChecked;


                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message.ToString(), "UserDataPersistentManager.LoadUserData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }

            _isLoaded = true;

        }
    }
}
