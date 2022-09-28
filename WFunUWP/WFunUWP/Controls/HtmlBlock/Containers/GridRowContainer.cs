using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WFunUWP.Controls.Containers
{
    internal class GridRowContainer : DocumentContainer<GridRow>
    {
        private int _columnIndex = 0;

        public GridRowContainer(GridRow ctrl) : base(ctrl)
        {
        }

        public override bool CanContain(DependencyObject ctrl)
        {
            return ctrl is GridColumn;
        }

        protected override void Add(DependencyObject ctrl)
        {
            if (Control.Container != null && _columnIndex >= Control.Container.ColumnDefinitions.Count)
            {
                Control.Container.ColumnDefinitions.Add(new ColumnDefinition());
            }

            GridColumn column = ctrl as GridColumn;

            column.Index = _columnIndex++;
            column.Row = Control;
        }
    }
}
