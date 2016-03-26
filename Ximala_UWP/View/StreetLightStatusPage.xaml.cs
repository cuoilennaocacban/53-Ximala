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

        public StreetLightStatusPage()
        {
            InitializeComponent();
        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}