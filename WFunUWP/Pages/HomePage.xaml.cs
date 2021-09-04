using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.BaseUri));
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div[2]/div/div/div/table/tbody");
            HtmlNodeCollection CNodes = node.ChildNodes;
            foreach (HtmlNode item in CNodes)
            {
                if (item.InnerHtml.Contains("td"))
                {
                    Collection.Add(new FeedListModel(item.InnerHtml));
                }
            }
        }
    }
}
