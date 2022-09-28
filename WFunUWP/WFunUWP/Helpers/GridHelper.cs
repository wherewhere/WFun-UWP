using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WFunUWP.Helpers
{
    public static class GridHelper
    {
        public static UIElement GetChild(this Grid grid, int column, int row)
        {
            return grid == null
                ? throw new ArgumentNullException("grid")
                : grid.Children.Count > 0
                ? grid.Children
                        .OfType<FrameworkElement>()
                        .FirstOrDefault(r => Grid.GetColumn(r) == column && Grid.GetRow(r) == row)
                : (UIElement)null;
        }

        public static T GetChild<T>(this Grid grid, int column, int row) where T : UIElement
        {
            return grid.GetChild(column, row) as T;
        }
    }
}
