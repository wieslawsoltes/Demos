using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpeedReader
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private MatchCollection _words;
        private int _currentWord = 0;
        private int _wordCount = 0;
        private bool _haveWords = false;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                sliderWords.Minimum = 0;
                sliderWords.Maximum = 0;
                sliderWords.Value = 0;

                _timer = new DispatcherTimer();
                _timer.Tick += (ts, te) => Tick();
            };

            buttonLoadRtf.Click += (s, e) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*",
                    FileName = ""
                };

                if (dlg.ShowDialog() == true)
                {
                    LoadRtf(dlg.FileName);
                }
            };

            buttonSaveRtf.Click += (s, e) =>
            {
                var dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*",
                    FileName = "document"
                };

                if (dlg.ShowDialog() == true)
                {
                    SaveRtf(dlg.FileName);
                }
            };

            buttonPlayPause.Click += (s, e) =>
            {
                if (_haveWords)
                {
                    if (_timer.IsEnabled)
                        Pause();
                    else
                        Play();
                }
            };

            buttonParseReset.Click += (s, e) =>
            {
                try
                {
                    if (_haveWords)
                    {
                        _wordCount = 0;
                        wordCounter.Text = "";

                        sliderWords.Minimum = 0;
                        sliderWords.Maximum = 0;
                        sliderWords.Value = 0;

                        _currentWord = 0;

                        text.Visibility = System.Windows.Visibility.Visible;
                        words.Visibility = System.Windows.Visibility.Hidden;

                        words.Text = "";

                        _haveWords = false;
                    }
                    else
                    {
                        string str = new TextRange(
                            text.Document.ContentStart,
                            text.Document.ContentEnd).Text;

                        int nchunks = int.Parse(chunks.Text);

                        _words = Regex.Matches(str, @"((?:(\S+\s+){0," + (nchunks - 1) + @"})\S+)");
                        _wordCount = _words.Count;

                        wordCounter.Text = _wordCount.ToString();

                        sliderWords.Minimum = 0;
                        sliderWords.Maximum = _wordCount - 1;
                        sliderWords.Value = 0;

                        _currentWord = 0;

                        text.Visibility = System.Windows.Visibility.Hidden;
                        words.Visibility = System.Windows.Visibility.Visible;

                        _haveWords = true;

                        words.Text = _words[_currentWord].Value;

                        sliderWords.Value = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message, 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            };

            sliderWords.ValueChanged += (s, e) =>
            {
                if (_haveWords)
                {
                    _currentWord = (int)sliderWords.Value;
                    words.Text = _words[_currentWord].Value;
                }
            };
        }

        private void LoadRtf(string fileName)
        {
            if(File.Exists(fileName))
            {
                try
                {
                    using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        new TextRange(
                            text.Document.ContentStart, 
                            text.Document.ContentEnd).Load(fs, DataFormats.Rtf);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message, 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }

        private void SaveRtf(string fileName)
        {
            try
            {
                using (var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    new TextRange(
                        text.Document.ContentStart, 
                        text.Document.ContentEnd).Save(fs, DataFormats.Rtf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void Play()
        {
            double wpmValue = double.Parse(wpm.Text);
            if (wpmValue > 0)
            {
                double delay = (1000.0 / (wpmValue / 60.0)) * int.Parse(chunks.Text);
                _timer.Interval = TimeSpan.FromMilliseconds(delay);
                _timer.Start();
            }
            else
            {
                MessageBox.Show(
                    "wpm must be greater than 0", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void Pause()
        {
            _timer.Stop();
        }


        private void Tick()
        {
            if (_haveWords)
            {
                if (_currentWord < _wordCount)
                {
                    sliderWords.Value = _currentWord;
                    _currentWord++;
                }
                else
                {
                    _timer.Stop();
                }
            }
        }
    }
}
