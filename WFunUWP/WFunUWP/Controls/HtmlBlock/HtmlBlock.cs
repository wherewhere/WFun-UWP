using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WFunUWP.Controls.Containers;
using WFunUWP.Controls.Writers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls
{
    public sealed class HtmlBlock : Control
    {
        private Grid _container;
        private DocumentStyle _docStyles;

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(HtmlBlock), new PropertyMetadata(null, SourcePropertyChanged));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlBlock self = d as HtmlBlock;
            self.UpdateContentAsync();
        }

        public static readonly DependencyProperty DocumentStyleProperty = DependencyProperty.Register("DocumentStyle", typeof(DocumentStyle), typeof(HtmlBlock), new PropertyMetadata(new DocumentStyle(), DocumentStylesChanged));

        public DocumentStyle DocumentStyle
        {
            get { return (DocumentStyle)GetValue(DocumentStyleProperty); }
            set { SetValue(DocumentStyleProperty, value); }
        }

        internal static readonly DependencyProperty DefaultDocumentStyleProperty = DependencyProperty.Register("DefaultDocumentStyle", typeof(DocumentStyle), typeof(HtmlBlock), new PropertyMetadata(new DocumentStyle(), DocumentStylesChanged));

        public DocumentStyle DefaultDocumentStyle
        {
            get { return (DocumentStyle)GetValue(DefaultDocumentStyleProperty); }
            set { SetValue(DefaultDocumentStyleProperty, value); }
        }

        private static void DocumentStylesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HtmlBlock self = d as HtmlBlock;

            self._docStyles.Reset(self);
            self._docStyles.Merge(self.DefaultDocumentStyle, self.DocumentStyle);
        }

        public HtmlBlock()
        {
            DefaultStyleKey = typeof(HtmlBlock);

            _docStyles = new DocumentStyle();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _container = GetTemplateChild("_container") as Grid;

            UpdateContentAsync();
        }

        private void UpdateContentAsync()
        {
            if (_container != null && !string.IsNullOrEmpty(Source))
            {
                _container.RowDefinitions.Clear();
                _container.ColumnDefinitions.Clear();
                _container.Children.Clear();

                GridDocumentContainer container = new GridDocumentContainer(_container);

                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(Source);

                    HtmlNode body = doc?.DocumentNode;

                    WriteFragments(body, container);
                }
                catch (Exception ex)
                {
                    ShowError(ex, container);
                }
            }
        }

        private void WriteFragments(HtmlNode fragment, DocumentContainer parentContainer)
        {
            if (parentContainer != null)
            {
                foreach (HtmlNode childFragment in fragment.ChildNodes)
                {
                    HtmlWriter writer = HtmlWriterFactory.Find(childFragment);

                    DependencyObject ctrl = writer?.GetControl(childFragment);

                    if (ctrl != null)
                    {
                        if (!parentContainer.CanContain(ctrl))
                        {
                            DocumentContainer antecesorContainer = parentContainer.Find(ctrl);

                            if (antecesorContainer == null)
                            {
                                continue;
                            }
                            else
                            {
                                parentContainer = antecesorContainer;
                            }
                        }

                        DocumentContainer currentContainer = parentContainer.Append(ctrl);

                        WriteFragments(childFragment, currentContainer);

                        writer?.ApplyStyles(_docStyles, ctrl, childFragment);
                    }
                }
            }
        }

        private static void ShowError(Exception ex, GridDocumentContainer gridContainer)
        {
            Paragraph p = new Paragraph();
            p.FontFamily = new FontFamily("Courier New");

            p.Inlines.Add(new Run
            {
                Text = $"Error rendering document: {ex.Message}"
            });
            gridContainer.Append(p);

            Debug.WriteLine($"HtmlBlock: Error rendering document. Ex: {ex}");
        }
    }
}
