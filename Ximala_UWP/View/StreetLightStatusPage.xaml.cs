﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using Ximala_UWP.ViewModel;

namespace Ximala_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreetLightStatusPage
    {
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public LightStatusViewModel Vm => (LightStatusViewModel)DataContext;

        private StreamSocket _socket;
        private StreamSocketListener _socketListener;

        private UsbSerial _usbSerial;
        private RemoteDevice _arduino;

        private RfcommDeviceService _service;

        public StreetLightStatusPage()
        {
            InitializeComponent();
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            //_usbSerial = new UsbSerial("VID_2341", "PID_0043");
            //_arduino = new RemoteDevice(_usbSerial);

            //_usbSerial.ConnectionEstablished += _usbSerial_ConnectionEstablished;
            //_usbSerial.begin(9600, SerialConfig.SERIAL_8N1);

            try
            {
                //Hello world
                Vm.ConnectionStatus = "Connecting...";
                var devices = await
                        DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));

                var device = devices.Single(x => x.Name == "HC-05");

                _service = await RfcommDeviceService.FromIdAsync(device.Id);

                _socket = new StreamSocket();

                await
                    _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                Vm.ConnectionStatus = _service.ConnectionHostName + " | " + _service.ConnectionServiceName;

                Vm.ConnectionStatus = "Start listener";

                _socketListener = new StreamSocketListener();

                //await _socketListener.BindServiceNameAsync(_service.ServiceId.AsString());
                //_socketListener.ConnectionReceived += _socketListener_ConnectionReceived;

                Task taskReceive = Task.Run(async () => { await ListenForMessagesAsync(_socket); });
                await taskReceive.ConfigureAwait(false);

                Vm.ConnectionStatus = "Listenning";
            }
            catch (Exception ex)
            {
                //tbError.Text = ex.Message;
                Vm.ConnectionStatus = ex.Message;
            }


        }

        private async Task ListenForMessagesAsync(StreamSocket localsocket)
        {
            while (localsocket != null)
            {
                try
                {
                    string message = String.Empty;
                    DataReader dataReader = new DataReader(localsocket.InputStream);
                    dataReader.InputStreamOptions = InputStreamOptions.Partial;

                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await dataReader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Read the string.
                    uint stringLength = dataReader.ReadUInt32();
                    uint actualStringLength = await dataReader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    Vm.ConnectionStatus = String.Format("Received data: \"{0}\"", dataReader.ReadString(actualStringLength));


                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void _usbSerial_ConnectionEstablished()
        {
            Vm.ConnectionStatus = "Connected via USB";
            _arduino.pinMode(13, PinMode.OUTPUT);
        }

        private async void _socketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Read the string.
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    Vm.ConnectionStatus = String.Format("Received data: \"{0}\"", reader.ReadString(actualStringLength));
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                Vm.ConnectionStatus = "Listening Failed: " + exception.Message;
            }
        }

        private async Task<uint> Send(string msg)
        {
            try
            {
                var writer = new DataWriter(_socket.OutputStream);

                writer.WriteString(msg);

                // Launch an async task to 
                //complete the write operation
                var store = writer.StoreAsync().AsTask();

                return await store;
            }
            catch (Exception ex)
            {
                //tbError.Text = ex.Message;
                Vm.ConnectionStatus = ex.Message;
                return 0;
            }
        }

        private async void OnButton_OnClick(object sender, RoutedEventArgs e)
        {
            //_arduino.digitalWrite(13, PinState.HIGH);
            var noOfCharsSent = await Send("a");

            if (noOfCharsSent != 0)
            {
                Vm.ConnectionStatus = "Send: " + noOfCharsSent;
            }
        }

        private async void OffButton_OnClick(object sender, RoutedEventArgs e)
        {
            var noOfCharsSent = await Send("b");

            if (noOfCharsSent != 0)
            {
                Vm.ConnectionStatus = "Send: " + noOfCharsSent;
            }
        }
    }
}