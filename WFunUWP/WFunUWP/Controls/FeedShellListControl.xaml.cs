﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task Refresh(int p = -1)
        {
            if (p == -2)
            {
                await ReplyDS.Refresh();
            }
            else
            {
                _ = await ReplyDS.LoadMoreItemsAsync(20);
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

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        protected override async Task<IList<FeedReplyModel>> LoadItemsAsync(uint count)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
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
