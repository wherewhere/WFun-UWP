using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using WFunUWP.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WFunUWP.Controls
{
    public sealed partial class FeedShellDetailControl : UserControl, INotifyPropertyChanged
    {
        private FeedDetailModel feedDetail;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public FeedDetailModel FeedDetail
        {
            get => feedDetail;
            set
            {
                feedDetail = value;
                _ = GenerateActivityAsync();
                RaisePropertyChangedEvent();
            }
        }

        internal Grid FeedArticleTitle { get => feedArticleTitle; }

        public FeedShellDetailControl() => InitializeComponent();

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (FeedDetail.IsCopyEnabled || (e != null && !UIHelper.IsOriginSource(sender, e.OriginalSource))) { return; }

            UIHelper.OpenLinkAsync((sender as FrameworkElement).Tag as string);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            void DisabledCopy()
            {
                if ((sender as FrameworkElement).DataContext is ICanCopy i)
                {
                    i.IsCopyEnabled = false;
                }
            }

            FrameworkElement element = sender as FrameworkElement;
            switch (element.Name)
            {
                case "ShareButton":
                    DisabledCopy();
                    break;

                default:
                    DisabledCopy();
                    UIHelper.OpenLinkAsync((sender as Button).Tag as string);
                    break;
            }
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            DataPackage dp = new DataPackage();
            dp.SetText(element.Tag.ToString());
            Clipboard.SetContent(dp);
        }

        public void Grid_Tapped(object sender, TappedRoutedEventArgs _)
        {
            if ((sender as FrameworkElement).Tag is string s)
            {
                UIHelper.OpenLinkAsync(s);
            }
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            moreButton.Flyout.ShowAt(this);
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                StackPanel_Tapped(sender, null);
            }
            else if (e.Key == Windows.System.VirtualKey.Menu)
            {
                moreButton.Flyout.ShowAt(this);
            }
        }

        private UserActivitySession _currentActivity;
        private async Task GenerateActivityAsync()
        {
            // Get the default UserActivityChannel and query it for our UserActivity. If the activity doesn't exist, one is created.
            UserActivityChannel channel = UserActivityChannel.GetDefault();
            UserActivity userActivity = await channel.GetOrCreateUserActivityAsync(Utils.GetMD5(FeedDetail.QRUrl));

            // Populate required properties
            userActivity.VisualElements.DisplayText = FeedDetail.MessageTitle;
            userActivity.VisualElements.AttributionDisplayText = FeedDetail.MessageTitle;
            userActivity.VisualElements.Description = FeedDetail.Message.CSStoString();
            userActivity.ActivationUri = new Uri(FeedDetail.QRUrl);

            //Save
            await userActivity.SaveAsync(); //save the new metadata

            // Dispose of any current UserActivitySession, and create a new one.
            _currentActivity?.Dispose();
            _currentActivity = userActivity.CreateSession();
        }
    }
}
