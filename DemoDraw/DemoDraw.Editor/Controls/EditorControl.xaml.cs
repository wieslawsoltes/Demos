using System.Windows.Controls;

namespace DemoDraw.Editor.Controls
{
    public partial class EditorControl : UserControl
    {
        private EditorInput Input;

        public EditorControl()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            Input = new EditorInput();

            // Mouse

            Layout.PreviewMouseLeftButtonDown += (s, e) => Input.Point(e, Surface);
            Layout.PreviewMouseRightButtonDown += (s, e) => Input.Cancel(e, Surface);
            Layout.PreviewMouseMove += (s, e) => Input.Move(e, Surface);

            // Keyboard

            Layout.KeyDown += (s, e) => Input.Type(e, Surface);

            // Focus

            Loaded += (s, e) =>
            {
                Layout.Focus();
                Zoom.Focus();
            };
        }
    }
}
