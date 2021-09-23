using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Models;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace WFunUWP.Helpers.Tasks
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            Uri  uri = new Uri(SettingsHelper.Get<string>(SettingsHelper.TileUrl));
            try { await GetData(uri); } catch { }

            deferral.Complete();
        }

        public static void UpdateTile()
        {
            Uri uri = new Uri(SettingsHelper.Get<string>(SettingsHelper.TileUrl));
            try { _ = GetData(uri); } catch { }
        }

        private static async Task<HtmlDocument> GetJson(Uri uri)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await NetworkHelper.GetHtmlAsync(uri));
            return doc;
        }

        public static async Task GetData(Uri uri)
        {
            HtmlDocument token = await GetJson(uri);
            if (token != null && token.TryGetNode("/html/body/main/div/div/div/div[2]/div/div/div/table/tbody", out HtmlNode node))
            {
                int i = 0;
                HtmlNodeCollection CNodes = node.ChildNodes;
                foreach (HtmlNode item in CNodes)
                {
                    if (i >= 5) { break; }
                    if (item.HasChildNodes)
                    {
                        UpdateTitle(GetFeedTile(item.InnerHtml));
                    }
                }
            }
        }

        private static void UpdateTitle(TileContent tileContent)
        {
            try
            {
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                TileNotification tileNotification = new TileNotification(tileContent.GetXml());
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
            catch { }
        }

        private static TileContent GetFeedTile(string token)
        {
            FeedListModel FeedDetail = new FeedListModel(token);
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    DisplayName = FeedDetail.UserName,
                    Arguments = FeedDetail.Url,

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = FeedDetail.MessageTitle.CSStoString(),
                                    HintStyle = AdaptiveTextStyle.Caption,
                                },

                                new AdaptiveText()
                                {
                                    Text = FeedDetail.Message,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true,
                                    HintMaxLines = 3
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 33,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = FeedDetail.UserAvatar.Uri,
                                                    HintCrop = AdaptiveImageCrop.Circle
                                                }
                                            },
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = FeedDetail.MessageTitle.CSStoString(),
                                                    HintStyle = AdaptiveTextStyle.Caption,
                                                },

                                                new AdaptiveText()
                                                {
                                                    Text = FeedDetail.Message,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintWrap = true,
                                                    HintMaxLines = 3
                                                }
                                            },
                                        }
                                    }
                                },
                            }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = FeedDetail.MessageTitle.CSStoString(),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintWrap = true,
                                },

                                new AdaptiveText()
                                {
                                    Text = FeedDetail.Message,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
