using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Codartis.SoftVis.UI.Wpf.Common.UIEvents;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Viewport.Viewing.Gestures
{
    /// <summary>
    /// Calculates translate changes when panning and zooming with keys.
    /// </summary>
    internal class KeyboardViewportGesture : ViewportGestureBase
    {
        private const int Acceleration = 3;
        private const int Deceleration = 9;
        private const int MaxSpeed = 50;
        private const int FramePerSeconds = 25;
        private const int FullZoomSeconds = 1;

        private readonly double _zoomAmountPerSpeed;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DispatcherTimer _timer;

        private int _zoomSpeed;
        private int _horizontalSpeed;
        private int _verticalSpeed;
        private readonly bool[] _isKeyDown = new bool[(int)GestureKeys.Max + 1];

        internal KeyboardViewportGesture(IDiagramViewport diagramViewport, IUIEventSource uiEventSource)
            : base(diagramViewport, uiEventSource)
        {
            var zoomRange = diagramViewport.MaxZoom - diagramViewport.MinZoom;
            _zoomAmountPerSpeed = zoomRange / (MaxSpeed / 2 * FramePerSeconds * FullZoomSeconds);

            UIEventSource.PreviewKeyDown += OnKeyDown;
            UIEventSource.PreviewKeyUp += OnKeyUp;

            _timer = CreateTimer(1000 / FramePerSeconds, OnTimerTick);
            _timer.Start();
        }

        private static DispatcherTimer CreateTimer(int intervalMillisec, EventHandler tickHandler)
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

            if (_zoomSpeed == 0)
                return;

            var zoomDirection = _zoomSpeed > 0 ? ZoomDirection.In : ZoomDirection.Out;

            if (IsZoomLimitReached(zoomDirection))
                _zoomSpeed = 0;
            else
                ZoomViewportBy(zoomDirection, Math.Abs(_zoomSpeed) * _zoomAmountPerSpeed);
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

        private int CalculateSpeed(int speed, GestureKeys positiveKey, GestureKeys negativeKey)
        {
            if (IsAcceleratingInPositiveDirection(speed, positiveKey, negativeKey))
            {
                speed += Acceleration;
            }
            else if (IsAccelaratingInNegativeDirection(speed, positiveKey, negativeKey))
            {
                speed -= Acceleration;
            }
            else if (speed < 0)
            {
                speed += CalculateDeceleration(speed);
            }
            else if (speed > 0)
            {
                speed -= CalculateDeceleration(speed);
            }

            speed = Math.Min(MaxSpeed, speed);
            speed = Math.Max(-MaxSpeed, speed);

            return speed;
        }

        private static int CalculateDeceleration(int speed)
        {
            return Math.Min(Math.Abs(speed), Deceleration);
        }

        private bool IsAcceleratingInPositiveDirection(int speed, GestureKeys positiveKey, GestureKeys negativeKey)
        {
            return _isKeyDown[(int)positiveKey]
                && !_isKeyDown[(int)negativeKey]
                && speed >= 0;
        }

        private bool IsAccelaratingInNegativeDirection(int speed, GestureKeys positiveKey, GestureKeys negativeKey)
        {
            return _isKeyDown[(int)negativeKey]
                && !_isKeyDown[(int)positiveKey]
                && speed <= 0;
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
            var gestureKey = KeyToGestureKey(key);
            if (gestureKey == null)
                return;

            _isKeyDown[(int)gestureKey.Value] = newState;
        }

        private static GestureKeys? KeyToGestureKey(Key key)
        {
            switch (key)
            {
                case Key.Up: return GestureKeys.Up;
                case Key.Down: return GestureKeys.Down;
                case Key.Left: return GestureKeys.Left;
                case Key.Right: return GestureKeys.Right;
                case Key.W: return GestureKeys.ZoomIn;
                case Key.S: return GestureKeys.ZoomOut;
                default: return null;
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
