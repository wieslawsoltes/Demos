using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RxCanvas.WPF
{
    public partial class MainWindow : Window
    {
        private DrawingView _view;
        private IDictionary<Tuple<Key, ModifierKeys>, Action> _shortcuts;

        public MainWindow()
        {
            InitializeComponent();

            InitializeDrawingView();
            InitlializeShortucts();
            Initialize();
        }

        private void InitializeDrawingView()
        {
            _view = new DrawingView(
                new Assembly[]
                {
                    typeof(Bootstrapper).GetTypeInfo().Assembly
                });
            _view.Initialize();
        }

        private void InitlializeShortucts()
        {
            _shortcuts = new Dictionary<Tuple<Key, ModifierKeys>, Action>();

            var keyConverter = new KeyConverter();
            var modifiersKeyConverter = new ModifierKeysConverter();

            _shortcuts.Add(
                new Tuple<Key, ModifierKeys>(
                    (Key)keyConverter.ConvertFromString("G"),
                    ModifierKeys.None),
                () => _view.ToggleSnap());

            _shortcuts.Add(
                new Tuple<Key, ModifierKeys>(
                    (Key)keyConverter.ConvertFromString("Delete"),
                    (ModifierKeys)modifiersKeyConverter.ConvertFromString("Control")),
                () => _view.Clear());

            foreach (var editor in _view.Editors)
            {
                var _editor = editor;
                _shortcuts.Add(
                    new Tuple<Key, ModifierKeys>(
                        (Key)keyConverter.ConvertFromString(editor.Key),
                        editor.Modifiers == "" ? ModifierKeys.None : (ModifierKeys)modifiersKeyConverter.ConvertFromString(editor.Modifiers)),
                    () => _view.Enable(_editor));
            }

            _shortcuts.Add(
                new Tuple<Key, ModifierKeys>(
                    (Key)keyConverter.ConvertFromString("Delete"),
                    ModifierKeys.None),
                () => _view.Delete());
        }

        private void Initialize()
        {
            for (int i = 0; i < _view.Layers.Count; i++)
            {
                Layout.Children.Add(_view.Layers[i].Native as UIElement);
            }

            _view.CreateGrid(600.0, 600.0, 30.0, 0.0, 0.0);

            PreviewKeyDown += (sender, e) =>
            {
                bool result = _shortcuts.TryGetValue(
                    new Tuple<Key, ModifierKeys>(e.Key, Keyboard.Modifiers),
                    out var action);

                if (result == true && action != null)
                {
                    action();
                }
            };

            DataContext = _view.Layers.LastOrDefault();
        }
    }
}
