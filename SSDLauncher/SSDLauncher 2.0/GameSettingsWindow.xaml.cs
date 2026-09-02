using global::SSDLauncher_2._0.ViewModels;
using Library.Models;
using SSDLauncher_2._0.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SSDLauncher_2._0
{
    public partial class GameSettingsWindow : Window
    {
        private readonly ControllerInputService _controller = new();

        public GameSettingsWindow(Game game)
        {
            InitializeComponent();
            DataContext = new GameSettingsViewModel(game);

            _controller.NavigateUp += () => MoveFocus(FocusNavigationDirection.Previous);
            _controller.NavigateDown += () => MoveFocus(FocusNavigationDirection.Next);
            _controller.Activate += ActivateFocusedElement;
            _controller.Back += Close;

            Loaded += (s, e) =>
            {
                _controller.Start();
                FocusFirstItem();
            };
            Closed += (s, e) => _controller.Stop();
        }

        private void MoveFocus(FocusNavigationDirection direction)
        {
            if (Keyboard.FocusedElement is UIElement focused)
            {
                focused.MoveFocus(new TraversalRequest(direction));
            }
            else
            {
                FocusFirstItem();
            }
        }

        private void ActivateFocusedElement()
        {
            if (Keyboard.FocusedElement is ButtonBase button)
            {
                button.Command?.Execute(button.CommandParameter);
            }
        }

        private void FocusFirstItem()
        {
            var firstRadio = VisualTreeHelpers.FindVisualChild<RadioButton>(ExecutablesList);
            firstRadio?.Focus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}


