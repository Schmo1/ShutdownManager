using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace ShutdownManager.Classes
{
    
    internal class UserDataPersistentManager : UserData
    {

        private const string jsonFileName = @"\UserData.json";

        private bool isLoaded = false;


        public void SaveUserData()
        {
            if (isLoaded) //UserData should first load, because at the beginnint the Values ar 0
            {
                try
                {
                    string path = Directory.GetCurrentDirectory() + jsonFileName;
                    StreamWriter sw = new StreamWriter(path, false);
                    UserData saveData = new UserData(Hours, Minutes, Seconds, TimerAction);
                    string userDataStr = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                    sw.Write(userDataStr);
                    sw.Close();
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.ToString(), "UserDataPersistentManager.SaveData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        public void LoadUserData()
        {
            string path = Directory.GetCurrentDirectory() + jsonFileName;

            if (File.Exists(path))
            {
                try
                {
                    StreamReader sr = new StreamReader(path);

                    UserData loadData = new UserData();
                    string userDataStr = sr.ReadToEnd();

                    loadData = JsonConvert.DeserializeObject<UserData>(userDataStr);
                    sr.Close();

                    Hours = loadData.Hours;
                    Minutes = loadData.Minutes;
                    Seconds = loadData.Seconds;
                    TimerAction = loadData.TimerAction;

                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "UserDataPersistentManager.LoadUserData", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }

            isLoaded = true;

        }
    }
}
