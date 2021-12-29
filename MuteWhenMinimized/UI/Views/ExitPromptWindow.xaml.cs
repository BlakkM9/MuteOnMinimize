using MuteOnMinimize.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MuteOnMinimize.UI.Views
{
    /// <summary>
    /// Interaction logic for ExitPromptWindow.xaml
    /// </summary>
    public partial class ExitPromptWindow : Window
    {
        public ExitPromptWindow()
        {
            InitializeComponent();
        }

        private void ToTrayClicked(object sender, RoutedEventArgs e)
        {
            if ((bool)saveDecision.IsChecked)
            {
                App.UserData.ExitChoice = ExitChoice.Tray;
            }
            Close();
            Owner.Hide();
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            if ((bool)saveDecision.IsChecked)
            {
                App.UserData.ExitChoice = ExitChoice.Exit;
            }
            Close();
            Application.Current.Shutdown();
        }
    }
}
