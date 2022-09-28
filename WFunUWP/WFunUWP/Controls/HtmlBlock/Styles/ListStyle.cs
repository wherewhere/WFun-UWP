using System;
using Windows.UI.Xaml;

namespace WFunUWP.Controls
{
    public class ListStyle : ParagraphStyle
    {
        public static readonly DependencyProperty BulletProperty = DependencyProperty.Register("Bullet", typeof(string), typeof(ListStyle), new PropertyMetadata(null));

        public string Bullet
        {
            get { return (string)GetValue(BulletProperty); }
            set { SetValue(BulletProperty, value); }
        }

        public void Merge(ListStyle style)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (!string.IsNullOrEmpty(style?.Bullet) && Bullet != style.Bullet)
            {
                Bullet = style.Bullet;
            }
            base.Merge(style);
        }
    }
}
