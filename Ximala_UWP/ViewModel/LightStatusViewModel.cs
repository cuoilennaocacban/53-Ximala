using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Ximala_UWP.Model;
using Ximala_UWP.Utilities;

namespace Ximala_UWP.ViewModel
{
    public class LightStatusViewModel : ObservableObject
    {

        private ObservableCollection<StreetLight> _streetLights;
        private string _connectionStatus;

        public ObservableCollection<StreetLight> StreetLights
        {
            get { return _streetLights; }
            set
            {
                if (Equals(value, _streetLights)) return;
                _streetLights = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                if (value == _connectionStatus) return;
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public void Initialize()
        {
            StreetLights = new ObservableCollection<StreetLight>();

            for (int i = 0; i < 3; i++)
            {
                StreetLight s = new StreetLight
                {
                    StreetLighId = i,
                    LightVisibility = Visibility.Collapsed
                };
                StreetLights.Add(s);
            }
        }
    }
}
