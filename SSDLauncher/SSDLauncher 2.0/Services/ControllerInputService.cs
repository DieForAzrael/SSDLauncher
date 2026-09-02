using SSDLauncher_2._0.Services;
using System;
using System.Windows.Threading;

namespace SSDLauncher_2._0.Services
{
    /// <summary>
    /// Periodically queries the first connected XInput-compatible controller
    /// (Xbox-style controllers), and raises events for directions/activations.
    /// Due to the DispatcherTimer, these events are already raised on the UI thread,
    /// so there's no need for Dispatcher.Invoke.
    /// </summary>
    public class ControllerInputService
    {
        private bool _isConnected;
        private readonly DispatcherTimer _timer;
        private ushort _previousButtons;
        public event Action? Connected;
        public event Action? Disconnected;
        public event Action? NavigateUp;
        public event Action? NavigateDown;
        public event Action? NavigateLeft;
        public event Action? NavigateRight;
        public event Action? Activate;
        public event Action? Back;

        public event Action? OpenSettings;

        public ControllerInputService()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
            _timer.Tick += Poll;
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        private void Poll(object? sender, EventArgs e)
        {
            var state = new XInputState();
            int result = XInputNative.XInputGetState(0, ref state);
            bool connectedNow = result == 0;

            if (connectedNow != _isConnected)
            {
                _isConnected = connectedNow;
                if (connectedNow) Connected?.Invoke();
                else Disconnected?.Invoke();
            }

            if (!connectedNow) return; // nincs csatlakoztatott kontroller a 0. porton

            ushort buttons = state.Gamepad.wButtons;
            ushort pressedNow = (ushort)(buttons & ~_previousButtons);

            if ((pressedNow & XInputNative.DPadUp) != 0) NavigateUp?.Invoke();
            if ((pressedNow & XInputNative.DPadDown) != 0) NavigateDown?.Invoke();
            if ((pressedNow & XInputNative.DPadLeft) != 0) NavigateLeft?.Invoke();
            if ((pressedNow & XInputNative.DPadRight) != 0) NavigateRight?.Invoke();
            if ((pressedNow & XInputNative.ButtonA) != 0) Activate?.Invoke();
            if ((pressedNow & XInputNative.ButtonY) != 0) OpenSettings?.Invoke();
            if ((pressedNow & XInputNative.ButtonB) != 0) Back?.Invoke();

            _previousButtons = buttons;
        }
    }
}