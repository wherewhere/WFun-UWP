﻿using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using InAppNotify = Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification;

namespace WFunUWP.Helpers
{
    public enum ImageType
    {
        Image,
        Avatar,
        Icon,
        Captcha,
    }

    internal static partial class ImageCacheHelper
    {
        private static readonly BitmapImage WhiteNoPicMode = new BitmapImage(new Uri("ms-appx:/Assets/NoPic/noavatar_small.jpg")) { DecodePixelHeight = 48, DecodePixelWidth = 48 };
        private static readonly BitmapImage DarkNoPicMode = new BitmapImage(new Uri("ms-appx:/Assets/NoPic/pic_loading.png")) { DecodePixelHeight = 200, DecodePixelWidth = 200 };
        internal static BitmapImage NoPic { get => ThemeHelper.IsDarkTheme() ? DarkNoPicMode : WhiteNoPicMode; }

        static ImageCacheHelper()
        {
            ImageCache.Instance.CacheDuration = TimeSpan.FromHours(8);
        }

        internal static async Task<BitmapImage> GetImageAsync(ImageType type, string url, InAppNotify notify = null)
        {
            if (string.IsNullOrEmpty(url)) { return NoPic; }

            if (url.IndexOf("ms-appx", StringComparison.Ordinal) == 0)
            {
                return new BitmapImage(new Uri(url));
            }
            else if (SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode))
            {
                return NoPic;
            }
            else
            {
                Uri uri = new Uri(url);

                try
                {
                    BitmapImage image = await ImageCache.Instance.GetFromCacheAsync(uri, true);
                    return image;
                }
                catch
                {
                    string str = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse().GetString("ImageLoadError");
                    if (notify == null)
                    {
                        UIHelper.ShowMessage(str);
                    }
                    else
                    {
                        _ = notify.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            notify.Show(str, UIHelper.Duration);
                        });
                    }
                    return NoPic;
                }
            }
        }

        internal static async Task<StorageFile> GetImageFileAsync(ImageType type, string url)
        {
            if (string.IsNullOrEmpty(url)) { return null; }

            if (url.IndexOf("ms-appx", StringComparison.Ordinal) == 0)
            {
                return await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
            }
            else if (SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode))
            {
                return null;
            }
            else
            {
                Uri uri = new Uri(url);
                return await ImageCache.Instance.GetFileFromCacheAsync(uri);
            }
        }

        internal static Task CleanCacheAsync()
        {
            return ImageCache.Instance.ClearAsync();
        }
    }

    internal static partial class ImageCacheHelper
    {
        [Obsolete]
        private static readonly Dictionary<ImageType, StorageFolder> folders = new Dictionary<ImageType, StorageFolder>();

        [Obsolete]
        internal static async Task<StorageFolder> GetFolderAsync(ImageType type)
        {
            StorageFolder folder;
            if (folders.ContainsKey(type))
            {
                folder = folders[type];
            }
            else
            {
                folder = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(type.ToString()) as StorageFolder;
                if (folder is null)
                {
                    folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(type.ToString(), CreationCollisionOption.OpenIfExists);
                }
                if (!folders.ContainsKey(type))
                {
                    folders.Add(type, folder);
                }
            }
            return folder;
        }

        [Obsolete]
        internal static async Task<BitmapImage> GetImageAsyncOld(ImageType type, string url, InAppNotify notify = null)
        {
            if (string.IsNullOrEmpty(url)) { return null; }

            if (url.IndexOf("ms-appx", StringComparison.Ordinal) == 0)
            {
                return new BitmapImage(new Uri(url));
            }
            else if (SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode))
            {
                return NoPic;
            }
            else
            {
                string fileName = Core.Helpers.Utils.GetMD5(url);
                StorageFolder folder = await GetFolderAsync(type);
                IStorageItem item = await folder.TryGetItemAsync(fileName);
                bool forceGetPic = false;
                if (item is null)
                {
                    StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    return await DownloadImageAsync(file, url, notify);
                }
                else
                {
                    return item is StorageFile file ? GetLocalImageAsync(file.Path, forceGetPic) : null;
                }
            }
        }

        [Obsolete]
        private static BitmapImage GetLocalImageAsync(string filename, bool forceGetPic)
        {
            try
            {
                return (filename is null || (!forceGetPic && SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode))) ? NoPic : new BitmapImage(new Uri(filename));
            }
            catch
            {
                return NoPic;
            }
        }

        [Obsolete]
        private static async Task<BitmapImage> DownloadImageAsync(StorageFile file, string url, InAppNotify notify)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                using (Stream stream = await hc.GetStreamAsync(new Uri(url)))
                using (Stream fs = await file.OpenStreamForWriteAsync())
                {
                    await stream.CopyToAsync(fs);
                }
                return new BitmapImage(new Uri(file.Path));
            }
            catch (FileLoadException) { return NoPic; }
            catch (HttpRequestException)
            {
                string str = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse().GetString("ImageLoadError");
                if (notify == null)
                {
                    UIHelper.ShowMessage(str);
                }
                else
                {
                    notify.Show(str, UIHelper.Duration);
                }
                return NoPic;
            }
        }

        [Obsolete]
        internal static async Task CleanOldVersionImageCacheAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                ImageType type = (ImageType)i;
                await (await GetFolderAsync(type)).DeleteAsync();
                await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(type.ToString());
            }
        }

        internal static async Task CleanCaptchaCacheAsync()
        {
#pragma warning disable 0612
            await (await GetFolderAsync(ImageType.Captcha)).DeleteAsync();
#pragma warning restore 0612
            await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Captcha");
        }
    }
}
