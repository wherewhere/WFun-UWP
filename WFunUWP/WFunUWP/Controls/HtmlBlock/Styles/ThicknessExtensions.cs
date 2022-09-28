using Windows.UI.Xaml;

namespace WFunUWP.Controls
{
    internal static class ThicknessExtensions
    {
        public static Thickness Merge(this Thickness thickness, Thickness thickness2)
        {
            return !double.IsNaN(thickness2.Top) && thickness != thickness2
                ? thickness2
                : double.IsNaN(thickness.Top) ? new Thickness() : thickness;
        }
    }
}
