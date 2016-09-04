using System.Windows.Controls;
using System.Windows.Media;

namespace DemoDraw.Editor.Controls
{
    public partial class PageControl : UserControl
    {
        private EditorContextRenderer Renderer;

        public PageControl()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Renderer = new EditorContextRenderer();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Renderer.Draw(dc, this);
        }
    }
}
