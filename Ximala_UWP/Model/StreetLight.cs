using Windows.UI.Xaml;
using Ximala_UWP.Utilities;

namespace Ximala_UWP.Model
{
    public class StreetLight : ObservableObject
    {
        private int _streetLighId;
        private Visibility _lightVisibility;

        public Visibility LightVisibility
        {
            get { return _lightVisibility; }
            set
            {
                if (value == _lightVisibility) return;
                _lightVisibility = value;
                OnPropertyChanged();
            }
        }

        public int StreetLighId
        {
            get { return _streetLighId; }
            set
            {
                if (value == _streetLighId) return;
                _streetLighId = value;
                OnPropertyChanged();
            }
        }
    }
}
