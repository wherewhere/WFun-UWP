using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Containers
{
    internal class SpanDocumentContainer : DocumentContainer<Span>
    {
        public SpanDocumentContainer(Span ctrl) : base(ctrl)
        {
        }

        public override bool CanContain(DependencyObject ctrl)
        {
            return ctrl is Inline;
        }

        protected override void Add(DependencyObject ctrl)
        {
            Inline inline = ctrl as Inline;
            Control.Inlines.Add(inline);
        }
    }
}
