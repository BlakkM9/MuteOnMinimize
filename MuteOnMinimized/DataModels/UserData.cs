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
            Converters = { new JsonStringEnumConverter() }
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

        public bool StartMinimized
        {
            get => _startMinimized;
            set
            {
                _startMinimized = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartMinimized)));
            }
        }
        private bool _startMinimized;

        public ExitChoice ExitChoice
        {
            get => _exitChoice;
            set
            {
                _exitChoice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExitChoice)));
            }
        }
        private ExitChoice _exitChoice;

        public HashSet<string> SavedProcesses { get; set; }


        /// <summary>
        /// Do not use to load <see cref="UserData"/>. Use <see cref="Load"/> instead.
        /// </summary>
        public UserData()
        {
            StartWithWindows = false;
            StartMinimized = false;
            ExitChoice = ExitChoice.None;
            SavedProcesses = new HashSet<string>();

            // Save whenever user data changed
            PropertyChanged += (s, e) => Save();
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


        private void Save()
        {
            string userDataString = JsonSerializer.Serialize(this, SERIALIZING_OPTIONS);
            File.WriteAllText(USER_DATA_PATH, userDataString);
        }
    }
}
