using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;



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
                string path = Directory.GetCurrentDirectory() + jsonFileName;

                StreamWriter sw = new StreamWriter(path, false);
                UserData saveData = new UserData(Hours, Minutes, Seconds, TimerAction);


                string userDataStr = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                sw.Write(userDataStr);
                sw.Close();
            }


        }

        public void LoadUserData()
        {
            string path = Directory.GetCurrentDirectory() + jsonFileName;

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);

                UserData loadData = new UserData();
                string userDataStr = sr.ReadToEnd();

                loadData = JsonConvert.DeserializeObject<UserData>(userDataStr);

                Hours = loadData.Hours;
                Minutes = loadData.Minutes;
                Seconds = loadData.Seconds;
                TimerAction = loadData.TimerAction;

                sr.Close();
            }

            isLoaded = true;

        }
    }
}
