﻿using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using WFunUWP.Helpers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace WFunUWP.Models
{
    public class ImageModel : INotifyPropertyChanged
    {
        protected WeakReference<BitmapImage> pic;
        public BitmapImage Pic
        {
            get
            {
                if (pic != null && pic.TryGetTarget(out BitmapImage image))
                {
                    return image;
                }
                else
                {
                    _ = GetImage();
                    return ImageCacheHelper.NoPic;
                }
            }
            protected set
            {
                if (pic == null)
                {
                    pic = new WeakReference<BitmapImage>(value);
                }
                else
                {
                    pic.SetTarget(value);
                }
                RaisePropertyChangedEvent();
            }
        }

        private bool isLongPic;
        public bool IsLongPic
        {
            get => isLongPic;
            private set
            {
                if (isLongPic != value)
                {
                    isLongPic = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        private bool isWidePic;
        public bool IsWidePic
        {
            get => isWidePic;
            private set
            {
                if (isWidePic != value)
                {
                    isWidePic = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        protected ImmutableArray<ImageModel> contextArray;
        public ImmutableArray<ImageModel> ContextArray
        {
            get => contextArray;
            set
            {
                if (contextArray.IsDefaultOrEmpty)
                {
                    contextArray = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        public bool IsGif => Uri.Substring(Uri.LastIndexOf('.')).ToUpperInvariant().Contains("GIF");

        private string uri;
        public string Uri
        {
            get => uri;
            set
            {
                if (uri != value)
                {
                    uri = value;
                    if (pic != null && pic.TryGetTarget(out BitmapImage _))
                    {
                        _ = GetImage();
                    }
                }
            }
        }

        private ImageType type;
        public ImageType Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    if (pic != null && pic.TryGetTarget(out BitmapImage _))
                    {
                        _ = GetImage();
                    }
                }
            }
        }

        public BitmapImage RealPic
        {
            get
            {
                if (pic != null && pic.TryGetTarget(out BitmapImage image))
                {
                    return image;
                }
                else
                {
                    GetImage().Wait();
                    return Pic;
                }
            }
        }

        public ImageModel(string uri, ImageType type)
        {
            Uri = uri;
            Type = type;
            ThemeHelper.UISettingChanged.Add(async mode =>
            {
                switch (mode)
                {
                    case UISettingChangedType.LightMode:
                    case UISettingChangedType.DarkMode:
                        _ = UIHelper.ShellDispatcher?.AwaitableRunAsync(() =>
                            {
                                if (SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode))
                                {
                                    if (pic != null && pic.TryGetTarget(out BitmapImage _))
                                    {
                                        Pic = ImageCacheHelper.NoPic;
                                    }
                                }
                            });
                        break;

                    case UISettingChangedType.NoPicChanged:
                        if (pic != null && pic.TryGetTarget(out BitmapImage _))
                        {
                            await GetImage();
                        }
                        break;
                }
            });
        }

        public event TypedEventHandler<ImageModel, object> LoadStarted;
        public event TypedEventHandler<ImageModel, object> LoadCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        private async Task GetImage()
        {
            LoadStarted?.Invoke(this, null);
            if (SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode)) { Pic = ImageCacheHelper.NoPic; }
            BitmapImage bitmapImage = await ImageCacheHelper.GetImageAsync(Type, Uri);
            Pic = bitmapImage;
            IsLongPic =
                bitmapImage.PixelHeight * Window.Current.Bounds.Width > bitmapImage.PixelWidth * Window.Current.Bounds.Height * 1.5
                && bitmapImage.PixelHeight > bitmapImage.PixelWidth * 1.5;
            IsWidePic =
                bitmapImage.PixelWidth * Window.Current.Bounds.Height > bitmapImage.PixelHeight * Window.Current.Bounds.Width * 1.5
                && bitmapImage.PixelWidth > bitmapImage.PixelHeight * 1.5;
            LoadCompleted?.Invoke(this, null);
        }
    }
}
