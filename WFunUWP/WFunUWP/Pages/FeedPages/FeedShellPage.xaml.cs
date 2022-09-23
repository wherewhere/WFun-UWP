using HtmlAgilityPack;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using WFunUWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FeedShellPage : Page
    {
        private FeedDetailModel FeedDetailModel;

        public FeedShellPage() => InitializeComponent();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UIHelper.ShowProgressBar();
            object[] vs = e.Parameter as object[];
            (bool isSucceed, HtmlDocument result) Results = (false, null);
            if (vs[0] is string id)
            {
                Results = await RequestHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetFeedDetail, id));
            }
            if (Results.isSucceed && Results.result.TryGetNode("/html/body/main/div/div/div", out HtmlNode node))
            {
                FeedDetailModel = new FeedDetailModel(node.InnerHtml);
                SetLayout();
            }
            UIHelper.HideProgressBar();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void SetLayout()
        {
            DetailControl.FeedDetail = FeedDetailModel;
            ListControl.ReplyDS = new Controls.ReplyDS(FeedDetailModel);
            TwoPaneView.MinWideModeWidth = FeedDetailModel?.IsFeedArticle ?? false ? 876 : 804;
            TwoPaneView.Pane1Length = new GridLength(FeedDetailModel?.IsFeedArticle ?? false ? 520 : 420);
            UIHelper.MainPage.SetTitle(FeedDetailModel.IsFeedArticle ? $"{FeedDetailModel.UserName}的图文" : $"{FeedDetailModel.UserName}的动态");
            _ = ListControl.Refresh(-2);
        }


        #region 界面模式切换

        private void TwoPaneView_ModeChanged(muxc.TwoPaneView sender, object args)
        {
            // Remove details content from it's parent panel.
            _ = (DetailControl.Parent as Panel).Children.Remove(DetailControl);

            // Single pane
            if (sender.Mode == muxc.TwoPaneViewMode.SinglePane)
            {
                // Add the details content to Pane1.
                Pane2Grid.Children.Add(DetailControl);
            }
            // Dual pane.
            else
            {
                // Put details content in Pane2.
                Pane1Grid.Children.Add(DetailControl);
            }
        }

        private void TwoPaneView_Loaded(object sender, RoutedEventArgs e)
        {
            TwoPaneView_ModeChanged(sender as muxc.TwoPaneView, null);
        }

        #endregion 界面模式切换
    }
}
