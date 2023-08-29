using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.IO;
using System.Reflection;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WFunUWP.Controls.Writers
{
    internal abstract class IFrameVideoWriter : HtmlWriter
    {
        private static readonly CoreCursor _arrowCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        private static readonly CoreCursor _handCursor = new CoreCursor(CoreCursorType.Hand, 1);

        protected abstract void SetScreenshot(ImageEx img, HtmlNode node);
        protected abstract ImageStyle GetStyle(DocumentStyle style);

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            if (fragment.NodeType == HtmlNodeType.Element)
            {
                HtmlNode node = fragment;
                if (node != null)
                {
                    Grid grid = new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    grid.Tapped += (sender, e) =>
                    {
                        Launcher.LaunchUriAsync(new Uri(GetIframeSrc(node))).AsTask().FireAndForget();
                    };

                    grid.PointerEntered += (sender, e) =>
                    {
                        Window.Current.CoreWindow.PointerCursor = _handCursor;
                    };
                    grid.PointerExited += (sender, e) =>
                    {
                        Window.Current.CoreWindow.PointerCursor = _arrowCursor;
                    };

                    AddColumn(grid);
                    AddColumn(grid);
                    AddColumn(grid);

                    Viewbox screenShot = GetImageControl((i) => SetScreenshot(i, node));

                    Grid.SetColumn(screenShot, 0);
                    Grid.SetColumnSpan(screenShot, 3);
                    grid.Children.Add(screenShot);

                    Viewbox player = GetImageControl((i) => i.Source = GetPlayerImage());

                    Grid.SetColumn(player, 1);
                    grid.Children.Add(player);

                    return grid;
                }
            }

            return null;
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            Grid grid = ctrl as Grid;
            Viewbox vb = grid.GetChild<Viewbox>(0, 0);

            ApplyImageStyles(vb, GetStyle(style));
        }

        protected static string GetIframeSrc(HtmlNode fragment)
        {
            if (fragment.NodeType == HtmlNodeType.Element)
            {
                HtmlNode node = fragment;
                if (node != null)
                {
                    return node.GetAttributeValue("src", string.Empty);
                }
            }
            return string.Empty;
        }

        protected static BitmapImage GetEmbebedImage(string name)
        {
            Assembly assembly = typeof(IFrameVideoWriter).GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    BitmapImage image = new BitmapImage();
                    image.SetSource(ms.AsRandomAccessStream());
                    return image;
                }
            }
        }

        private static BitmapImage GetPlayerImage()
        {
            return GetEmbebedImage("WFunUWP.Controls.HtmlBlock.PlayButton.png");
        }

        private static Viewbox GetImageControl(Action<ImageEx> setSource)
        {
            Viewbox viewbox = new Viewbox
            {
                StretchDirection = StretchDirection.DownOnly
            };

            ImageEx image = new ImageEx
            {
                Stretch = Stretch.Uniform,
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.Transparent)
            };
            setSource(image);
            viewbox.Child = image;

            return viewbox;
        }

        private static void AddColumn(Grid grid)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        }
    }
}
