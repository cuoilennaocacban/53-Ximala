using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
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

        private RfcommDeviceService _service;

        public StreetLightStatusPage()
        {
            InitializeComponent();
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Hello world
                Vm.ConnectionStatus = "Connecting...";
                var devices =
                    await
                        DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));

                var device = devices.Single(x => x.Name == "HC-05");

                _service = await RfcommDeviceService.FromIdAsync(
                    device.Id);

                _socket = new StreamSocket();

                await
                    _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                Vm.ConnectionStatus = _service.ConnectionHostName + " | " + _service.ConnectionServiceName;
            }
            catch (Exception ex)
            {
                //tbError.Text = ex.Message;
                Vm.ConnectionStatus = ex.Message;
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