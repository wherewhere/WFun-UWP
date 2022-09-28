﻿using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls
{
    public class TextStyle : DependencyObject
    {
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(TextStyle), new PropertyMetadata(null));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(TextStyle), new PropertyMetadata(null));

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        internal static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(TextStyle), new PropertyMetadata(0));

        internal double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeRatioProperty = DependencyProperty.Register("FontSizeRatio", typeof(string), typeof(TextStyle), new PropertyMetadata(null));

        public string FontSizeRatio
        {
            get { return (string)GetValue(FontSizeRatioProperty); }
            set { SetValue(FontSizeRatioProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(TextStyle), new PropertyMetadata(null));

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(TextStyle), new PropertyMetadata(FontWeights.Normal));

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public void Reset(Control host)
        {
            BindingOperations.SetBinding(this, TextStyle.FontSizeProperty, CreateBinding(host, "FontSize"));
            BindingOperations.SetBinding(this, TextStyle.FontFamilyProperty, CreateBinding(host, "FontFamily"));
            BindingOperations.SetBinding(this, TextStyle.FontStyleProperty, CreateBinding(host, "FontStyle"));
            BindingOperations.SetBinding(this, TextStyle.FontWeightProperty, CreateBinding(host, "FontWeight"));
            BindingOperations.SetBinding(this, TextStyle.ForegroundProperty, CreateBinding(host, "Foreground"));
        }

        public void Merge(TextStyle style)
        {
            if (style != null)
            {
                if (style.FontFamily != null && FontFamily != style.FontFamily)
                {
                    FontFamily = style.FontFamily;
                }
                if (!string.IsNullOrEmpty(style.FontSizeRatio) && FontSizeRatio != style.FontSizeRatio)
                {
                    FontSizeRatio = style.FontSizeRatio;
                }
                if (style.FontStyle != FontStyle.Normal && FontStyle != style.FontStyle)
                {
                    FontStyle = style.FontStyle;
                }
                if (style.FontWeight.Weight != FontWeights.Normal.Weight && FontWeight.Weight != style.FontWeight.Weight)
                {
                    FontWeight = style.FontWeight;
                }
                if (style.Foreground != null && Foreground != style.Foreground)
                {
                    Foreground = style.Foreground;
                }
            }
        }

        public float FontSizeRatioValue()
        {
            return float.TryParse(FontSizeRatio, out float resultRatio) ? resultRatio : 0;
        }

        private static Binding CreateBinding(object source, string path)
        {
            return new Binding
            {
                Path = new PropertyPath(path),
                Source = source
            };
        }
    }
}
