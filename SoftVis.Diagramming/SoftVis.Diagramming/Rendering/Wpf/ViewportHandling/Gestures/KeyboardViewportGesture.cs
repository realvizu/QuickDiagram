using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling.Gestures
{
    /// <summary>
    /// Calculates translate changes when panning and zooming with keys.
    /// </summary>
    internal class KeyboardViewportGesture : ViewportGestureBase
    {
        private const int _acceleration = 3;
        private const int _deceleration = 9;
        private const int _maxSpeed = 50;
        private const int _minSpeed = -_maxSpeed;
        private const int _framePerSeconds = 25;
        private const int _fullZoomSeconds = 1;

        private readonly double _zoomAmountPerSpeed;
        private readonly DispatcherTimer _timer;

        private int _zoomSpeed = 0;
        private int _horizontalSpeed = 0;
        private int _verticalSpeed = 0;
        private bool[] _isKeyDown = new bool[(int)GestureKeys.Max + 1];

        public KeyboardViewportGesture(IViewportHost viewportHost)
            : base(viewportHost)
        {
            var zoomRange = viewportHost.MaxZoom - viewportHost.MinZoom;
            _zoomAmountPerSpeed = zoomRange / (_maxSpeed/2 * _framePerSeconds * _fullZoomSeconds);

            ViewportHost.PreviewKeyDown += OnKeyDown;
            ViewportHost.PreviewKeyUp += OnKeyUp;

            _timer = CreateTimer(1000 / _framePerSeconds, OnTimerTick);
            _timer.Start();
        }

        private DispatcherTimer CreateTimer(int intervalMillisec, EventHandler tickHandler)
        {
            var timer = new DispatcherTimer();
            timer.Tick += tickHandler;
            timer.Interval = new TimeSpan(0, 0, 0, 0, intervalMillisec);
            return timer;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            ProcessZoom();
            ProcessTranslate();
        }

        private void ProcessZoom()
        {
            _zoomSpeed = CalculateSpeed(_zoomSpeed, GestureKeys.ZoomIn, GestureKeys.ZoomOut);

            if (_zoomSpeed != 0)
            {
                var zoomDirection = _zoomSpeed > 0 ? ZoomDirection.In : ZoomDirection.Out;

                if (IsZoomLimitReached(zoomDirection))
                {
                    _zoomSpeed = 0;
                }
                else
                {
                    ZoomViewportBy(zoomDirection, Math.Abs(_zoomSpeed) * _zoomAmountPerSpeed);
                }
            }
        }

        private void ProcessTranslate()
        {
            _verticalSpeed = CalculateSpeed(_verticalSpeed, GestureKeys.Down, GestureKeys.Up);
            _horizontalSpeed = CalculateSpeed(_horizontalSpeed, GestureKeys.Right, GestureKeys.Left);

            if (_horizontalSpeed != 0 || _verticalSpeed != 0)
            {
                MoveViewportCenterInDiagramSpaceBy(new Vector(_horizontalSpeed, _verticalSpeed));
            }
        }

        private int CalculateSpeed(int speed, GestureKeys positiveDirection, GestureKeys negativeDirection)
        {
            if (_isKeyDown[(int)positiveDirection] && !(_isKeyDown[(int)negativeDirection]) && speed >= 0)
            {
                speed += _acceleration;
            }
            else if (_isKeyDown[(int)negativeDirection] && !(_isKeyDown[(int)positiveDirection]) && speed <= 0)
            {
                speed -= _acceleration;
            }
            else if (speed < 0)
            {
                speed += Math.Min(Math.Abs(speed), _deceleration);
            }
            else if (speed > 0)
            {
                speed -= Math.Min(Math.Abs(speed), _deceleration);
            }

            speed = Math.Min(_maxSpeed, speed);
            speed = Math.Max(_minSpeed, speed);

            return speed;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            SetKeyDownState(e.Key, true);
            e.Handled = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            SetKeyDownState(e.Key, false);
            e.Handled = true;
        }

        private void SetKeyDownState(Key key, bool newState)
        {
            switch (key)
            {
                case (Key.Up):
                    _isKeyDown[(int)GestureKeys.Up] = newState;
                    break;
                case (Key.Down):
                    _isKeyDown[(int)GestureKeys.Down] = newState;
                    break;
                case (Key.Left):
                    _isKeyDown[(int)GestureKeys.Left] = newState;
                    break;
                case (Key.Right):
                    _isKeyDown[(int)GestureKeys.Right] = newState;
                    break;
                case (Key.W):
                    _isKeyDown[(int)GestureKeys.ZoomIn] = newState;
                    break;
                case (Key.S):
                    _isKeyDown[(int)GestureKeys.ZoomOut] = newState;
                    break;
                default:
                    break;
            }
        }

        private enum GestureKeys
        {
            Up = 0,
            Down,
            Left,
            Right,
            ZoomIn,
            ZoomOut,
            Max = ZoomOut
        }
    }
}
