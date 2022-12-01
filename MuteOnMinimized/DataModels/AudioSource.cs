using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Management;

namespace MuteOnMinimize.DataModels
{
    public class AudioSource : INotifyPropertyChanged
    {

        public Process Process { get; }
        public BitmapSource Icon { get; }

        public bool IsMuted
        {
            get => App.AudioManager.IsMuted(Process.Id);
            set
            {
                App.AudioManager.MuteSource(value, Process.Id);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMuted)));
            }
        }

        public bool MuteOnFocusLoss {
            get => _muteOnFocusLoss;
            set
            {
                _muteOnFocusLoss = value;
                IsMuted = _muteOnFocusLoss;

                if (_muteOnFocusLoss)
                {
                    App.UserData.SavedProcesses.Add(Process.ProcessName);
                }
                else
                {
                    App.UserData.SavedProcesses.Remove(Process.ProcessName);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MuteOnFocusLoss)));
            }
        }
        private bool _muteOnFocusLoss;



        public AudioSource(int pid)
        {
            Process = Process.GetProcessById(pid);


            string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + Process.Id;
            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQueryString);
            using ManagementObjectCollection results = searcher.Get();

            string path = string.Empty;
            foreach (ManagementObject mo in results)
            {
                path = (string)mo["ExecutablePath"];
                break;
            }

            if (System.IO.File.Exists(path))
            {
                Bitmap bmp = System.Drawing.Icon.ExtractAssociatedIcon(path).ToBitmap();
                Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmp.GetHbitmap(),
                   IntPtr.Zero,
                   System.Windows.Int32Rect.Empty,
                   BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}