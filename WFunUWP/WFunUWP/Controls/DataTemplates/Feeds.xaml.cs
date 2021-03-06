using Microsoft.Toolkit.Uwp.UI.Extensions;
using WFunUWP.Helpers;
using WFunUWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WFunUWP.Controls.DataTemplates
{
    public partial class Feeds : ResourceDictionary
    {
        public Feeds() => InitializeComponent();

        internal static void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement s = sender as FrameworkElement;
            if (e != null && !UIHelper.IsOriginSource(sender, e.OriginalSource)) { return; }
            if ((s.DataContext as ICanCopy)?.IsCopyEnabled ?? false) { return; }

            if (e != null) { e.Handled = true; }

            UIHelper.OpenLinkAsync(s.Tag as string);
        }

        internal static void FeedButton_Click(object sender, RoutedEventArgs _)
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
                    UIHelper.OpenLinkAsync((sender as FrameworkElement).Tag as string);
                    break;
            }
        }

        internal static void ListViewItem_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (UIHelper.IsOriginSource(sender, e.OriginalSource))
            {
                if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
                {
                    OnTapped(sender, null);
                }
                else if (e.Key == Windows.System.VirtualKey.Menu)
                {
                    ListViewItem_RightTapped(sender, null);
                }
            }
        }

        internal static void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs _)
        {
            FrameworkElement s = (FrameworkElement)sender;
            Button b = s.FindName("MoreButton") as Button;
            //b.Flyout.ShowAt(s);
        }

        internal static void Flyout_Opened(object sender, object _)
        {
            Flyout Flyout = (Flyout)sender;
            if (Flyout.Content == null)
            {
                Flyout.Content = new ShowQRCodeControl
                {
                    QRCodeText = (string)Flyout.Target.Tag
                };
            }
        }

        internal static void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UserControl uc = sender as UserControl;
            StackPanel bp = uc.FindChildByName("BtnsPanel") as StackPanel;
            double width = e is null ? uc.Width : e.NewSize.Width;
            bp.SetValue(Grid.RowProperty, width > 600 ? 0 : 4);
        }

        internal static void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UserControl_SizeChanged(sender, null);
        }

        internal static void RelaRLis_ItemClick(object _, ItemClickEventArgs e)
        {
            UIHelper.OpenLinkAsync(((RelationRowsItem)e.ClickedItem).Url);
        }
    }
}
