using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using NmeaParser;
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


        private DispatcherTimer _timer;
        private bool _isStillReading = false;

        private UsbSerial _usbSerial;
        private BluetoothSerial _bluetoothSerial;
        private RemoteDevice _arduino;

        private RfcommDeviceService _service;

        public StreetLightStatusPage()
        {
            InitializeComponent();
            SetupTimer();
            //Loaded += StreetLightStatusPage_Loaded;
            //_timer.Start();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            _timer.Tick += _timer_Tick;
        }

        private async void _timer_Tick(object sender, object e)
        {
            if (_isStillReading)
            {
                return;
            }

            
            Task taskReceive = Task.Run(async () =>
            {
                await ListenForMessagesAsync(_socket);
            });
            //await taskReceive.ConfigureAwait(false);
            
        }

        private async void StreetLightStatusPage_Loaded(object sender, RoutedEventArgs e)
        {
            Task taskReceive = Task.Run(async () => { await ListenForMessagesAsync(_socket); });
            await taskReceive.ConfigureAwait(false);
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            //_usbSerial = new UsbSerial("VID_2341", "PID_0043");
            //_bluetoothSerial = new BluetoothSerial("HC-05");
            //_arduino = new RemoteDevice(_bluetoothSerial);

            //_bluetoothSerial.ConnectionEstablished += _usbSerial_ConnectionEstablished;
            //_arduino.DeviceReady += _arduino_DeviceReady;
            //_bluetoothSerial.begin(9600, SerialConfig.SERIAL_8N1);

            try
            {
                //Guid btUuid = Guid.Parse("6f5b75e8-be27-4684-a776-3238826d1a91");
                //string deviceFilter = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(btUuid));
                //var devices = await DeviceInformation.FindAllAsync(deviceFilter);

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

                //RfcommServiceProvider provider = await RfcommServiceProvider.CreateAsync(RfcommServiceId.SerialPort);

                //_socketListener = new StreamSocketListener();
                //_socketListener.ConnectionReceived += _socketListener_ConnectionReceived;
                //await _socketListener.BindServiceNameAsync(_service.ServiceId.AsString(),
                //    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                ////InitializeServiceSdpAttributes(rfcommProvider);
                //provider.StartAdvertising(_socketListener);

                //if (_service != null)
                //{
                //    var rangeFinder = new NmeaParser.BluetoothDevice(_service);
                //    rangeFinder.MessageReceived += device_NmeaMessageReceived;
                //    await rangeFinder.OpenAsync();
                //}

                Vm.ConnectionStatus = "Listenning";
            }
            catch (Exception ex)
            {
                //tbError.Text = ex.Message;
                Vm.ConnectionStatus = ex.Message;
            }


        }

        private void _arduino_DeviceReady()
        {
            Vm.ConnectionStatus = "Device Ready";
            _arduino.pinMode(13, PinMode.OUTPUT);
        }

        private void device_NmeaMessageReceived(object sender, NmeaMessageReceivedEventArgs e)
        {
        }

        private async Task ListenForMessagesAsync(StreamSocket localsocket)
        {
            while (localsocket != null)
            {
                try
                {
                    Debug.WriteLine("Start Reading");
                    _isStillReading = true;
                    string message = String.Empty;
                    DataReader dataReader = new DataReader(localsocket.InputStream);
                    dataReader.InputStreamOptions = InputStreamOptions.Partial;

                    // Read first 4 bytes (length of the subsequent string).
                    //uint sizeFieldCount = await dataReader.LoadAsync(4);
                    //if (sizeFieldCount != sizeof (uint))
                    //{
                    //    // The underlying socket was closed before we were able to read the whole data.
                    //    return;
                    //}

                    // Read the string.
                    uint stringLength = dataReader.ReadUInt32();
                    uint actualStringLength = await dataReader.LoadAsync(stringLength);
                    //if (stringLength != actualStringLength)
                    //{
                    //    // The underlying socket was closed before we were able to read the whole data.
                    //    return;
                    //}

                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            Vm.ConnectionStatus = String.Format("Received data: \"{0}\"",
                                dataReader.ReadString(stringLength));
                        });
                    _isStillReading = false;
                }
                catch (Exception ex)
                {
                    _isStillReading = false;
                }
            }
        }

        private void _usbSerial_ConnectionEstablished()
        {
            Vm.ConnectionStatus = "Connected via Bluetooth";
            //_arduino.pinMode(13, PinMode.OUTPUT);
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
            //_arduino.digitalRead(13);
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