using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WFunUWP.Helpers;
using WFunUWP.Helpers.DataSource;
using WFunUWP.Models;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WFunUWP.Controls
{
    public sealed partial class FeedShellListControl : UserControl, INotifyPropertyChanged
    {
        private ReplyDS replyDS;

        public ScrollViewer ScrollViewer;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public ReplyDS ReplyDS
        {
            get => replyDS;
            set
            {
                replyDS = value;
                RaisePropertyChangedEvent();
            }
        }

        public FeedShellListControl() => InitializeComponent();

        public object ListViewHeader { get => Head.Content; set => Head.Content = value; }

        public IncrementalLoadingTrigger IncrementalLoadingTrigger { get => ListView.IncrementalLoadingTrigger; set => ListView.IncrementalLoadingTrigger = value; }

        public ScrollMode VerticalScrollMode
        {
            get => (ScrollMode)(ScrollViewer?.VerticalScrollMode);

            set
            {
                if (ScrollViewer != null)
                {
                    ScrollViewer.VerticalScrollMode = value;
                }
            }
        }

        private void ListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ScrollViewer = VisualTree.FindDescendantByName(ListView, "ScrollViewer") as ScrollViewer;
        }

        public async Task Refresh(int p = -1)
        {
            if (p == -2)
            {
                await ReplyDS?.Refresh();
            }
            else
            {
                _ = await ReplyDS?.LoadMoreItemsAsync(20);
            }
        }
    }

    /// <summary>
    /// Provide list of News. <br/>
    /// You can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more.
    /// </summary>
    public class ReplyDS : DataSourceBase<FeedReplyModel>
    {
        private int _loaditems;
        private FeedDetailModel _FeedDetail;

        internal ReplyDS(FeedDetailModel FeedDetail)
        {
            _FeedDetail = FeedDetail;
            OnLoadMoreStarted += UIHelper.ShowProgressBar;
            OnLoadMoreCompleted += UIHelper.HideProgressBar;
        }

        protected override async Task<IList<FeedReplyModel>> LoadItemsAsync(uint count)
        {
            if (_currentPage == 1)
            {
                _loaditems = 0;
            }
            if (_loaditems == _FeedDetail.ReplyList.Count())
            {
                return null;
            }
            else if (_loaditems + count > _FeedDetail.ReplyList.Count())
            {
                List<FeedReplyModel> results = _FeedDetail.ReplyList.ToList().GetRange(_loaditems, _FeedDetail.ReplyList.Count() - _loaditems);
                _loaditems = _FeedDetail.ReplyList.Count();
                await Task.Delay(500);
                return results;
            }
            else
            {
                List<FeedReplyModel> results = _FeedDetail.ReplyList.ToList().GetRange(_loaditems, (int)count);
                _loaditems += (int)count;
                return results;
            }
        }

        protected override void AddItems(IList<FeedReplyModel> items)
        {
            if (items != null)
            {
                foreach (FeedReplyModel news in items)
                {
                    Add(news);
                }
            }
        }
    }
}
