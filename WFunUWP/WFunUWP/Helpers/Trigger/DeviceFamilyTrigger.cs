using Windows.System.Profile;
using Windows.UI.Xaml;

namespace WFunUWP.Helpers.Trigger
{
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _actualDeviceFamily;
        private string _triggerDeviceFamily;

        public string DeviceFamily
        {
            get { return _triggerDeviceFamily; }
            set
            {
                _triggerDeviceFamily = value;
                _actualDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                SetActive(_triggerDeviceFamily == _actualDeviceFamily);
            }
        }
    }
}
