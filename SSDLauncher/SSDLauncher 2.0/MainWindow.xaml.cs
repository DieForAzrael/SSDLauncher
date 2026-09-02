using Library.Models;
using SSDLauncher_2._0.Services;
using SSDLauncher_2._0.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SSDLauncher_2._0
{
    public partial class MainWindow : Window
    {
        private readonly ControllerInputService _controller = new();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => FocusFirstCard();

            if (DataContext is MainViewModel vm)
            {
                vm.Games.CollectionChanged += (s, e) =>
                {
                    FocusFirstCard();
                    ((App)System.Windows.Application.Current).ShowMainWindow();
                };
            }

            _controller.NavigateUp += () => NavigateFocus(FocusNavigationDirection.Up);
            _controller.NavigateDown += () => NavigateFocus(FocusNavigationDirection.Down);
            _controller.NavigateLeft += () => NavigateFocus(FocusNavigationDirection.Left);
            _controller.NavigateRight += () => NavigateFocus(FocusNavigationDirection.Right);
            _controller.Activate += ActivateFocusedCard;
            _controller.OpenSettings += OpenSettingsForFocusedCard;

            _controller.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!((App)System.Windows.Application.Current).IsExiting)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                base.OnClosing(e);
            }
        }

        private void NavigateFocus(FocusNavigationDirection direction)
        {
            if (!IsActive) return; 

            if (Keyboard.FocusedElement is UIElement focused)
            {
                focused.MoveFocus(new TraversalRequest(direction));
            }
            else
            {
                FocusFirstCard();
            }
        }

        private void ActivateFocusedCard()
        {
            if (!IsActive) return;

            if (Keyboard.FocusedElement is Button button)
            {
                button.Command?.Execute(button.CommandParameter);
            }
        }

        private void OpenSettingsForFocusedCard()
        {
            if (!IsActive) return;

            if (Keyboard.FocusedElement is FrameworkElement element &&
                element.DataContext is Game game &&
                DataContext is MainViewModel vm)
            {
                vm.OpenGameSettingsCommand.Execute(game);
            }
        }

        private void FocusFirstCard()
        {
            var firstButton = VisualTreeHelpers.FindVisualChild<Button>(GamesItemsControl);
            firstButton?.Focus();
        }
    }
}