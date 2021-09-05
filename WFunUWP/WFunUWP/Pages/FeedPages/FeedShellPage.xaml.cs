using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace WFunUWP.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FeedShellPage : Page, INotifyPropertyChanged
    {
        private FeedDetailModel FeedDetailModel;
        private double[] VerticalOffsets { get; set; } = new double[3];

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public FeedShellPage() => InitializeComponent();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HtmlDocument doc = new HtmlDocument();
            object[] vs = e.Parameter as object[];
            if (vs[0] is string id)
            {
                doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetFeedDetail, id)));
            }
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div");
            FeedDetailModel = new FeedDetailModel(node.InnerHtml);
            SetLayout();
            if (MainScrollMode == ScrollMode.Disabled)
            {
                _ = DetailScrollViewer.ChangeView(null, VerticalOffsets[1], null, true);
                _ = RightScrollViewer.ChangeView(null, VerticalOffsets[2], null, true);
            }
            else
            {
                _ = MainScrollViewer.ChangeView(null, VerticalOffsets[0], null, true);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (MainScrollMode == ScrollMode.Disabled)
            {
                VerticalOffsets[1] = DetailScrollViewer.VerticalOffset;
                VerticalOffsets[2] = RightScrollViewer.VerticalOffset;
            }
            else
            {
                VerticalOffsets[0] = MainScrollViewer.VerticalOffset;
            }
            base.OnNavigatingFrom(e);
        }

        private void SetLayout()
        {
            DetailControl.FeedDetail = FeedDetailModel;

            Page_SizeChanged(null, null);
        }


        #region 界面模式切换

        private double detailListHeight;
        private ScrollMode mainScrollMode = ScrollMode.Auto;

        private double DetailListHeight
        {
            get => detailListHeight;
            set
            {
                detailListHeight = value;
                RaisePropertyChangedEvent();
            }
        }

        public ScrollMode MainScrollMode
        {
            get => mainScrollMode;
            set
            {
                mainScrollMode = value;
                RaisePropertyChangedEvent();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DetailControl.FeedArticleTitle != null)
            {
                DetailControl.FeedArticleTitle.Height = DetailControl.FeedArticleTitle.Width * 0.44;
            }

            void SetDualPanelMode()
            {
                MainScrollMode = ScrollMode.Disabled;
                DetailListHeight = e?.NewSize.Height ?? Window.Current.Bounds.Height;

                RightColumnDefinition.Width = new GridLength(1, GridUnitType.Star);
                DetailBorder.Padding = new Thickness(0, (double)Application.Current.Resources["PageTitleHeight"], 0, 16);
                DetailBorder.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Auto);
                MainGrid.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Disabled);
                MainGrid.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                MainGrid.Padding = new Thickness(0);
                MainGrid.Margin = new Thickness(0);
                RightScrollViewer.SetValue(Grid.ColumnProperty, 1);
                RightScrollViewer.SetValue(Grid.RowProperty, 0);
                RightGrid.Padding = (Thickness)Application.Current.Resources["StackPanelMargin"];
                RightGrid.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Auto);
                RightGrid.InvalidateArrange();
            }

            if ((e?.NewSize.Width ?? ActualWidth) >= 804 && !(FeedDetailModel?.IsFeedArticle ?? false))
            {
                LeftColumnDefinition.Width = new GridLength(420);
                SetDualPanelMode();
            }
            else if ((e?.NewSize.Width ?? ActualWidth) >= 876 && (FeedDetailModel?.IsFeedArticle ?? false))
            {
                LeftColumnDefinition.Width = new GridLength(520);
                SetDualPanelMode();
            }
            else
            {
                MainScrollMode = ScrollMode.Auto;
                DetailListHeight = double.NaN;
                MainGrid.Margin = new Thickness(0, 0, 0, 0);
                MainGrid.Padding = (Thickness)Application.Current.Resources["StackPanelMargin"];
                MainGrid.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Auto);
                MainGrid.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
                DetailBorder.Padding = RightGrid.Padding = new Thickness(0, 0, 0, 12);
                DetailBorder.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Disabled);
                LeftColumnDefinition.Width = new GridLength(1, GridUnitType.Star);
                RightColumnDefinition.Width = new GridLength(0);
                RightScrollViewer.SetValue(Grid.ColumnProperty, 0);
                RightScrollViewer.SetValue(Grid.RowProperty, 1);
                RightGrid.SetValue(ScrollViewer.VerticalScrollModeProperty, ScrollMode.Disabled);
            }
        }

        #endregion 界面模式切换
    }
}
