using RemoteShutdown;
using RemoteShutdown.CommandResolving;
using RemoteShutdown.Functionalty;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XSLibrary.Network.Connections;
using XSLibrary.Utility;

namespace RemoteControlClientWPF
{
    public partial class Control : UserControl
    {
        class StatusLogger : Logger
        {
            Control m_control;

            public StatusLogger(Control control)
            {
                m_control = control;
            }

            protected override void LogMessage(string text)
            {
                m_control.Dispatcher.Invoke(() => m_control.m_status.Log(text));
            }
        }

        StatusLogger Logger { get; set; }
        DataReceiver Receiver { get; set; }

        public Control(TCPPacketConnection connection)
        {
            InitializeComponent();

            Logger = new StatusLogger(this);
            Logger.LogLevel = LogLevel.Error;

            Logger.Log(LogLevel.Priority, "Connected to server.");

            List<ICommandResolver> commandResolver = new List<ICommandResolver>();
            commandResolver.Add(new ShutdownCommandResolve(new ShutdownHandler() { Logger = Logger }));
            commandResolver.Add(new VolumeCommandResolver(new VolumeHandler() { Logger = Logger }));
            commandResolver.Add(new MediaPlayerResolver(new MediaPlayerHandler() { Logger = Logger }));
            commandResolver.Add(new ServerCommandResolver(new ServerCommandHandler() { Logger = Logger }));

            Receiver = new DataReceiver(connection, new CommandoExecutionActor(commandResolver));
            Receiver.Logger = Logger;
            Receiver.Run();
        }

        private void OnDisconnectClick(object sender, RoutedEventArgs e)
        {
            Receiver.Dispose();
        }
    }
}
