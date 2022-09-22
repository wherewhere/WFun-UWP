using HtmlAgilityPack;
using System.Collections.ObjectModel;
using WFunUWP.Controls.Dialogs;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using WFunUWP.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        internal ObservableCollection<object> Collection = new ObservableCollection<object>();

        public HomePage() => InitializeComponent();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UIHelper.ShowProgressBar();
            (bool isSucceed, HtmlDocument result) Results = await RequestHelper.GetHtmlAsync(UriHelper.BaseUri);
            if (Results.isSucceed && Results.result.TryGetNode("/html/body/main/div/div/div/div[2]/div/div/div/table/tbody", out HtmlNode node) && node.HasChildNodes)
            {
                HtmlNodeCollection CNodes = node.ChildNodes;
                foreach (HtmlNode item in CNodes)
                {
                    if (item.HasChildNodes)
                    {
                        Collection.Add(new FeedListModel(item.InnerHtml));
                    }
                }
            }
            UIHelper.HideProgressBar();
        }
    }
}
