﻿using CommonTools.Lib.fx45.ExceptionTools;
using CommonTools.Lib.fx45.InputTools;
using CommonTools.Lib.fx45.ViewModelTools;
using CommonTools.Lib.ns11.ExceptionTools;
using CommonTools.Lib.ns11.InputTools;
using CommonTools.Lib.ns11.SignalRHubServers;
using CommonTools.Lib.ns11.StringTools;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace CommonTools.Lib.fx45.SignalRServers
{
    public class SignalRServerToggleVM : ViewModelBase
    {
        private ISignalRServerSettings _cfg;
        private ISignalRWebApp         _app;
        private CommonLogListVM        _log;

        public SignalRServerToggleVM(ISignalRWebApp signalRWebApp,
                                     ISignalRServerSettings signalRServerSettings,
                                     CommonLogListVM commonLogListVM)
        {
            _app = signalRWebApp;
            _cfg = signalRServerSettings;
            _log = commonLogListVM;

            StartServerCmd = R2Command.Relay(StartServer);
            StopServerCmd  = R2Command.Async(StopServer);

            StatusChanged += (s, e) => _log.Add(e);
        }

        public IR2Command  StartServerCmd  { get; }
        public IR2Command  StopServerCmd   { get; }


        private void StartServer()
        {
            var url = _cfg.ServerURL;

            SetStatus($"Starting server at [{url}] ...");
            try
            {
                _app.StartServer(url);
                SetStatus("Server successfully started.");
            }
            catch (TargetInvocationException ex)
            {
                var msg = GetPortConflictMessage(url);
                MessageBox.Show(msg, "Failed to start server", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.ShowAlert();
            }
            catch (Exception ex)
            {
                SetStatus(ex.Info(true, true));
            }
        }


        private async Task StopServer()
        {
            SetStatus("Stopping server ...");
            _app.StopServer();
            await Task.Delay(1000);
            SetStatus("Server successfully stopped.");
        }


        private string GetPortConflictMessage(string serverUrl)
        {
            return $"Unable to start server at {serverUrl}"
            + L.F + "You may need to pick a different port number."
            + L.F + "Other Options:"
            + L.f + "1.)  Run the server as Administrator."
            + L.f + "2.)  netsh http add urlacl http://*:123456/ user=EVERYONE";
        }
    }
}
