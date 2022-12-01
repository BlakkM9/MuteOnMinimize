using MuteOnMinimize.DataModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace MuteOnMinimize.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<AudioSource> AudioSources { get; private set; }


        public MainWindow()
        {
            InitializeComponent();

            AudioSources = new ObservableCollection<AudioSource>();
            sourceListView.ItemsSource = AudioSources;

            startWithWindowsCheckBox.DataContext = App.UserData;

            App.FocusManager.FocusChanged += FocusChangedHandler;

            InitAudioManager();
        }


        #region Initialization

        private void InitAudioManager()
        {
            App.AudioManager.SourceAdded += pid =>
            {
                AudioSource src = new AudioSource(pid);
                if (App.UserData.SavedProcesses.Contains(src.Process.ProcessName))
                {
                    src.MuteOnFocusLoss = true;
                }
                AudioSources.Add(src);
            };

            App.AudioManager.SourceRemoved += pid =>
            {
                int index = -1;

                for (int i = 0; i < AudioSources.Count; i++)
                {
                    if (AudioSources[i].Process.Id == pid)
                    {
                        index = i;
                        break;
                    }
                }

                AudioSources.RemoveAt(index);
            };

            App.AudioManager.LoadAudioSourcesAsync();
        }

        #endregion


        #region EventHandlers

        private void FocusChangedHandler(int newFocusPid)
        {

            foreach (AudioSource s in AudioSources)
            {

                if (s.Process.Id == newFocusPid)
                {

                    if (s.MuteOnFocusLoss)
                    {
                        s.IsMuted = false;
                    }
                }
                else
                {
                    if (s.MuteOnFocusLoss)
                    {
                        s.IsMuted = true;
                    }
                }
            }
        }


        private void WindowClosingHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (App.UserData.ExitChoice)
            {
                case ExitChoice.None:
                    e.Cancel = true;
                    ExitPromptWindow prompt = new ExitPromptWindow
                    {
                        Owner = this
                    };
                    prompt.ShowDialog();
                    break;
                case ExitChoice.Tray:
                    e.Cancel = true;
                    Hide();
                    break;
            }
        }


        private void WindowClosedHandler(object sender, EventArgs e)
        {
            foreach (AudioSource src in AudioSources)
            {
                if (src.MuteOnFocusLoss && src.IsMuted)
                {
                    src.IsMuted = false;
                }
            }

            Debug.WriteLine("MainWindowClosed");
        }

        #endregion
    }
}
