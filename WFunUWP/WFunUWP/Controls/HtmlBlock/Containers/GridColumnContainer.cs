using System.Linq;
using WFunUWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Containers
{
    internal class GridColumnContainer : DocumentContainer<GridColumn>
    {
        public GridColumnContainer(GridColumn ctrl) : base(ctrl)
        {
        }

        public override bool CanContain(DependencyObject ctrl)
        {
            return !(ctrl is GridRow || ctrl is GridColumn);
        }

        protected override void Add(DependencyObject ctrl)
        {
            if (Control.Row != null && Control.Row.Container != null)
            {
                if (ctrl is FrameworkElement)
                {
                    AddChild(ctrl as FrameworkElement);
                }
                else if (ctrl is Block)
                {
                    RichTextBlock textBlock = FindOrCreateTextBlock();

                    textBlock.Blocks.Add(ctrl as Block);
                }
                else if (ctrl is Inline)
                {
                    RichTextBlock textBlock = FindOrCreateTextBlock();
                    Paragraph p = FindOrCreateParagraph(textBlock);

                    p.Inlines.Add(ctrl as Inline);
                }
            }
        }

        private static Paragraph FindOrCreateParagraph(RichTextBlock textBlock)
        {
            Paragraph p = textBlock.Blocks
                                .OfType<Paragraph>()
                                .LastOrDefault();

            if (p == null)
            {
                p = new Paragraph();
                textBlock.Blocks.Add(p);
            }

            return p;
        }

        private RichTextBlock FindOrCreateTextBlock()
        {
            Border border = Control.Row.Container.GetChild<Border>(Control.Index, Control.Row.Index);
            Grid grid = border?.Child as Grid;
            RichTextBlock textBlock = grid?.GetChild<RichTextBlock>(0, grid.RowDefinitions.Count - 1);
            if (textBlock == null)
            {
                textBlock = new RichTextBlock();
                AddChild(textBlock);
            }
            return textBlock;
        }

        private void AddChild(FrameworkElement element)
        {
            Border border = Control.Row.Container.GetChild<Border>(Control.Index, Control.Row.Index);
            if (border == null)
            {
                border = new Border
                {
                    Child = new Grid()
                };

                Grid.SetColumn(border, Control.Index);
                Grid.SetRow(border, Control.Row.Index);
                if (Control.ColSpan > 0)
                {
                    Grid.SetColumnSpan(border, Control.ColSpan);
                }
                if (Control.RowSpan > 0)
                {
                    Grid.SetRowSpan(border, Control.RowSpan);
                }

                Control.Row.Container.Children.Add(border);
            }

            Grid grid = border.Child as Grid;

            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });
            Grid.SetRow(element, grid.RowDefinitions.Count - 1);

            grid.Children.Add(element);
        }
    }
}
