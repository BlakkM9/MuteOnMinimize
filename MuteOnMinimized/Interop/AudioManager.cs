using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MuteOnMinimize.Interop
{
    public class AudioManager : IDisposable
    {

        private AudioSessionManager2 _sessionManager;
        private Dictionary<int, AudioSessionControl> _sessionControls;

        public event Action<int> SourceAdded;
        public event Action<int> SourceRemoved;

        public AudioManager()
        {
            _sessionControls = new Dictionary<int, AudioSessionControl>();
        }


        public async void LoadAudioSourcesAsync()
        {
            await Task.Run(() =>
            {
                _sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render);
            });

            _sessionManager.SessionCreated += SessionCreated;

            using AudioSessionEnumerator sessionEnumerator = _sessionManager.GetSessionEnumerator();
            foreach (AudioSessionControl session in sessionEnumerator)
            {
                AddSession(session);
            }
        }


        private void SessionCreated(object sender, SessionCreatedEventArgs e)
        {
            AddSession(e.NewSession);
        }


        private void SessionExpired(AudioSessionControl session)
        {
            RemoveSession(session);
        }


        private void AddSession(AudioSessionControl session)
        {
            int pid = GetPIDForSession(session);
            _sessionControls.Add(pid, session);

            session.StateChanged += (sender, args) =>
            {
                if (args.NewState == AudioSessionState.AudioSessionStateExpired)
                {
                    SessionExpired(session);
                }
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                SourceAdded?.Invoke(pid);
            });
        }


        private void RemoveSession(AudioSessionControl session)
        {
            int pid = GetPIDForSession(session);
            _sessionControls.Remove(pid);

            Application.Current.Dispatcher.Invoke(() =>
            {
                SourceRemoved?.Invoke(pid);
            });
        }


        public void MuteSource(bool mute, int pid)
        {
            using SimpleAudioVolume simpleVolume = _sessionControls[pid].QueryInterface<SimpleAudioVolume>();
            simpleVolume.IsMuted = mute;
        }


        public bool IsMuted(int pid)
        {
            using SimpleAudioVolume simpleVolume = _sessionControls[pid].QueryInterface<SimpleAudioVolume>();
            return simpleVolume.IsMuted;
        }


        private static int GetPIDForSession(AudioSessionControl session)
        {
            using AudioSessionControl2 sessionControl = session.QueryInterface<AudioSessionControl2>();
            return sessionControl.ProcessID;
        }


        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            using MMDevice device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia);

            //Debug.WriteLine("DefaultDevice: " + device.FriendlyName);

            AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device);
            return sessionManager;
        }


        #region IDisposable

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (AudioSessionControl ctrl in _sessionControls.Values)
                    {
                        ctrl.Dispose();
                    }
                    _sessionManager.Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}