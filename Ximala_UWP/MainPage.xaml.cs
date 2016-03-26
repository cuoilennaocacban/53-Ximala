using Windows.UI.Xaml;
using Ximala_UWP.View;

namespace Ximala_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void StreetLightStatusButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (StreetLightStatusPage));
        }
    }
}
