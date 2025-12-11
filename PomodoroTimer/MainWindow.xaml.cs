using PomodoroTimer.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (MainViewModel)DataContext;

            _notifyIcon = new Forms.NotifyIcon();
            _notifyIcon.Icon = ExtractAppIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.DoubleClick += (s, args) => RestoreWindow();

            var contextMenu = new Forms.ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (s, e) => RestoreWindow());
            contextMenu.Items.Add("Exit", null, (s, e) => CloseApp());
            _notifyIcon.ContextMenuStrip = contextMenu;

            this.StateChanged += OnWindowStateChanged;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            UpdateTrayText();
        }

        private Icon ExtractAppIcon()
        {
            try
            {
                return new Icon("Resources/appIcon.ico");
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized && _viewModel.IsTimerRunning)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.TimerText) ||
                e.PropertyName == nameof(MainViewModel.IsBreakMode))
            {
                UpdateTrayText();
            }

            if (e.PropertyName == nameof(MainViewModel.IsTimerRunning))
            {
                if (!_viewModel.IsTimerRunning)
                {
                    _notifyIcon.ShowBalloonTip(3000, "Pomodoro", "Time is up!", Forms.ToolTipIcon.Info);

                    RestoreWindow();
                }
            }
        }

        private void UpdateTrayText()
        {
            if (_notifyIcon.Visible)
            {
                string mode = _viewModel.IsBreakMode ? "Break" : "Focus";
                string text = $"{_viewModel.TimerText} - {mode}";

                if (text.Length >= 63) text = text.Substring(0, 63);
                _notifyIcon.Text = text;
            }
        }

        private void RestoreWindow()
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            this.ShowInTaskbar = true;

            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void CloseApp()
        {
            _notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon.Dispose();
            base.OnClosed(e);
        }
    }
}