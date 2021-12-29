using Hardcodet.Wpf.TaskbarNotification;
using MuteOnMinimize.DataModels;
using MuteOnMinimize.Interop;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MuteOnMinimize
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static readonly string APPLICATION_PATH = Process.GetCurrentProcess().MainModule.FileName;
        public static readonly string APPLICATION_NAME = "MuteOnMinimize";
        public static readonly string ICON_NAME = "MuteOnMinimize.ico";

        private TaskbarIcon _taskbarIcon;

        public static UserData UserData { get; private set; }
        public static AudioManager AudioManager { get; private set; }
        public static FocusManager FocusManager { get; private set; }

        private void ApplicationStartupHandler(object sender, StartupEventArgs e)
        {
            // Only allow a single instance of this process except started with multinstance argument
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1 && !e.Args.Contains("-multinstance"))
            {
                MessageBox.Show("Application is already running.");
                Current.Shutdown();
                return;
            }

            InitApp();
        }


        private void InitApp()
        {
            _taskbarIcon = (TaskbarIcon)FindResource("NotifyIcon");
            UserData = UserData.Load();
            AudioManager = new AudioManager();
            FocusManager = new FocusManager();
        }


        private void ApplicationExitHandler(object sender, ExitEventArgs e)
        {
            AudioManager?.Dispose();
            FocusManager?.Dispose();
            _taskbarIcon?.Dispose();
            if (UserData != null)
            {
                UserData.Save(UserData);
            }
        }


        private void ExitClickedHandler(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }


        private void TrayMouseDoubleClickHandler(object sender, RoutedEventArgs e)
        {
            MainWindow.Show();
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }
        }
    }
}
