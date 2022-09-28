using System.Linq;
using WFunUWP.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Containers
{
    internal class GridDocumentContainer : DocumentContainer<Grid>
    {
        public GridDocumentContainer(Grid ctrl) : base(ctrl)
        {
        }

        public override bool CanContain(DependencyObject ctrl)
        {
            return !(ctrl is GridColumn);
        }

        protected override void Add(DependencyObject ctrl)
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
            else if (ctrl is GridRow)
            {
                Control.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
                GridRow row = ctrl as GridRow;

                row.Index = Control.RowDefinitions.Count - 1;
                row.Container = Control;
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
            RichTextBlock textBlock = Control.GetChild<RichTextBlock>(0, Control.RowDefinitions.Count - 1);

            if (textBlock == null)
            {
                textBlock = new RichTextBlock();
                AddChild(textBlock);
            }
            return textBlock;
        }

        private void AddChild(FrameworkElement element)
        {
            Control.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });
            Grid.SetRow(element, Control.RowDefinitions.Count - 1);

            Control.Children.Add(element);
        }
    }
}
