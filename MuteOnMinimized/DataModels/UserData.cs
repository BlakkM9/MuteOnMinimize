using MuteOnMinimize.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MuteOnMinimize.DataModels
{
    public enum ExitChoice
    {
        None,
        Tray,
        Exit
    }


    public class UserData : INotifyPropertyChanged
    {
        public const string USER_DATA_PATH = "UserData.json";

        private static readonly JsonSerializerOptions SERIALIZING_OPTIONS = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters =
                {
                    new JsonStringEnumConverter()
                }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set
            {
                _startWithWindows = value;

                if (_startWithWindows)
                {
                    Utils.CreateStartupShortcut(App.APPLICATION_NAME, App.APPLICATION_PATH);
                }
                else
                {
                    Utils.RemoveStartupShortcut(App.APPLICATION_NAME, App.APPLICATION_PATH);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartWithWindows)));
            }
        }
        private bool _startWithWindows;

        public ExitChoice ExitChoice { get; set; }

        public HashSet<string> SavedProcesses { get; set; }


        public UserData()
        {
            StartWithWindows = false;
            ExitChoice = ExitChoice.None;
            SavedProcesses = new HashSet<string>();
        }


        public static UserData Load()
        {
            if (File.Exists(USER_DATA_PATH))
            {
                string userDataString = File.ReadAllText(USER_DATA_PATH);
                return JsonSerializer.Deserialize<UserData>(userDataString, SERIALIZING_OPTIONS);
            }
            else
            {
                return new UserData();
            }
        }


        public static void Save(UserData data)
        {
            string userDataString = JsonSerializer.Serialize(data, SERIALIZING_OPTIONS);
            File.WriteAllText(USER_DATA_PATH, userDataString);
        }
    }
}
