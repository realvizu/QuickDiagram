using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Behaviors
{
    /// <summary>
    /// Subscribes to the associated object's keystroke events and translates them to pan and zoom commands.
    /// </summary>
    internal class KeyboardPanAndZoomBehavior : PanAndZoomBehaviorBase
    {
        private const double AccelerationDefault = 3;
        private const double DecelerationDefault = 9;
        private const double MaxSpeedDefault = 50;
        private const int FramePerSeconds = 25;

        private DispatcherTimer _timer;
        private double _zoomSpeed;
        private double _horizontalSpeed;
        private double _verticalSpeed;
        private readonly bool[] _isKeyDown = new bool[(int)GestureKeys.Max + 1];

        public static readonly DependencyProperty PanUpKeyProperty =
            DependencyProperty.Register("PanUpKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty PanDownKeyProperty =
            DependencyProperty.Register("PanDownKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty PanLeftKeyProperty =
            DependencyProperty.Register("PanLeftKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty PanRightKeyProperty =
            DependencyProperty.Register("PanRightKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty ZoomInKeyProperty =
            DependencyProperty.Register("ZoomInKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty ZoomOutKeyProperty =
            DependencyProperty.Register("ZoomOutKey", typeof(Key), typeof(KeyboardPanAndZoomBehavior));

        public static readonly DependencyProperty PanAccelerationProperty =
            DependencyProperty.Register("PanAcceleration", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(AccelerationDefault));

        public static readonly DependencyProperty PanDecelerationProperty =
            DependencyProperty.Register("PanDeceleration", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(DecelerationDefault));

        public static readonly DependencyProperty PanMaxSpeedProperty =
            DependencyProperty.Register("PanMaxSpeed", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(MaxSpeedDefault));

        public static readonly DependencyProperty ZoomAccelerationProperty =
            DependencyProperty.Register("ZoomAcceleration", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(AccelerationDefault));

        public static readonly DependencyProperty ZoomDecelerationProperty =
            DependencyProperty.Register("ZoomDeceleration", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(DecelerationDefault));

        public static readonly DependencyProperty ZoomMaxSpeedProperty =
            DependencyProperty.Register("ZoomMaxSpeed", typeof(double), typeof(KeyboardPanAndZoomBehavior),
                new PropertyMetadata(MaxSpeedDefault));

        public Key PanUpKey
        {
            get { return (Key)GetValue(PanUpKeyProperty); }
            set { SetValue(PanUpKeyProperty, value); }
        }

        public Key PanDownKey
        {
            get { return (Key)GetValue(PanDownKeyProperty); }
            set { SetValue(PanDownKeyProperty, value); }
        }

        public Key PanLeftKey
        {
            get { return (Key)GetValue(PanLeftKeyProperty); }
            set { SetValue(PanLeftKeyProperty, value); }
        }

        public Key PanRightKey
        {
            get { return (Key)GetValue(PanRightKeyProperty); }
            set { SetValue(PanRightKeyProperty, value); }
        }

        public Key ZoomInKey
        {
            get { return (Key)GetValue(ZoomInKeyProperty); }
            set { SetValue(ZoomInKeyProperty, value); }
        }

        public Key ZoomOutKey
        {
            get { return (Key)GetValue(ZoomOutKeyProperty); }
            set { SetValue(ZoomOutKeyProperty, value); }
        }

        public double PanAcceleration
        {
            get { return (double)GetValue(PanAccelerationProperty); }
            set { SetValue(PanAccelerationProperty, value); }
        }

        public double PanDeceleration
        {
            get { return (double)GetValue(PanDecelerationProperty); }
            set { SetValue(PanDecelerationProperty, value); }
        }

        public double PanMaxSpeed
        {
            get { return (double)GetValue(PanMaxSpeedProperty); }
            set { SetValue(PanMaxSpeedProperty, value); }
        }

        public double ZoomAcceleration
        {
            get { return (double)GetValue(ZoomAccelerationProperty); }
            set { SetValue(ZoomAccelerationProperty, value); }
        }

        public double ZoomDeceleration
        {
            get { return (double)GetValue(ZoomDecelerationProperty); }
            set { SetValue(ZoomDecelerationProperty, value); }
        }

        public double ZoomMaxSpeed
        {
            get { return (double)GetValue(ZoomMaxSpeedProperty); }
            set { SetValue(ZoomMaxSpeedProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewKeyDown += OnKeyDown;
            AssociatedObject.PreviewKeyUp += OnKeyUp;

            _timer = CreateTimer(1000 / FramePerSeconds, OnTimerTick);
            _timer.Start();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            _timer.Stop();

            AssociatedObject.PreviewKeyDown -= OnKeyDown;
            AssociatedObject.PreviewKeyUp -= OnKeyUp;
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
            ProcessPan();
        }

        private void ProcessZoom()
        {
            _zoomSpeed = CalculateSpeed(_zoomSpeed, ZoomAcceleration, ZoomDeceleration, ZoomMaxSpeed,
                GestureKeys.ZoomIn, GestureKeys.ZoomOut);

            if (_zoomSpeed.IsEqualWithTolerance(0))
                return;

            var zoomDirection = _zoomSpeed > 0 ? ZoomDirection.In : ZoomDirection.Out;
            var zoomCenter = new Point(AssociatedObject.ActualWidth / 2, AssociatedObject.ActualHeight / 2);

            if (IsZoomLimitReached(zoomDirection))
                _zoomSpeed = 0;
            else
                Zoom(zoomDirection, Math.Abs(_zoomSpeed), zoomCenter);
        }

        private void ProcessPan()
        {
            _verticalSpeed = CalculateSpeed(_verticalSpeed, PanAcceleration, PanDeceleration, PanMaxSpeed, 
                GestureKeys.Down, GestureKeys.Up);

            _horizontalSpeed = CalculateSpeed(_horizontalSpeed, PanAcceleration, PanDeceleration, PanMaxSpeed,
                GestureKeys.Right, GestureKeys.Left);

            if (!_horizontalSpeed.IsEqualWithTolerance(0) || !_verticalSpeed.IsEqualWithTolerance(0))
            {
                Pan(new Vector(_horizontalSpeed, _verticalSpeed));
            }
        }

        private double CalculateSpeed(double speed, double acceleration, double deceleration, double maxSpeed,
            GestureKeys positiveKey, GestureKeys negativeKey)
        {
            if (IsAcceleratingInPositiveDirection(speed, positiveKey, negativeKey))
            {
                speed += acceleration;
            }
            else if (IsAccelaratingInNegativeDirection(speed, positiveKey, negativeKey))
            {
                speed -= acceleration;
            }
            else if (speed < 0)
            {
                speed += CalculateDeceleration(speed, deceleration);
            }
            else if (speed > 0)
            {
                speed -= CalculateDeceleration(speed, deceleration);
            }

            speed = Math.Min(maxSpeed, speed);
            speed = Math.Max(-maxSpeed, speed);

            return speed;
        }

        private static double CalculateDeceleration(double speed, double deceleration)
        {
            return Math.Min(Math.Abs(speed), deceleration);
        }

        private bool IsAcceleratingInPositiveDirection(double speed, GestureKeys positiveKey, GestureKeys negativeKey)
        {
            return _isKeyDown[(int)positiveKey]
                && !_isKeyDown[(int)negativeKey]
                && speed >= 0;
        }

        private bool IsAccelaratingInNegativeDirection(double speed, GestureKeys positiveKey, GestureKeys negativeKey)
        {
            return _isKeyDown[(int)negativeKey]
                && !_isKeyDown[(int)positiveKey]
                && speed <= 0;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            SetKeyDownState(e.Key, true);

            if (e.Key != Key.Tab)
                e.Handled = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            SetKeyDownState(e.Key, false);

            if (e.Key != Key.Tab)
                e.Handled = true;
        }

        private void SetKeyDownState(Key key, bool newState)
        {
            var gestureKey = KeyToGestureKey(key);
            if (gestureKey == null)
                return;

            _isKeyDown[(int)gestureKey.Value] = newState;
        }

        private GestureKeys? KeyToGestureKey(Key key)
        {
            if (key == PanUpKey) return GestureKeys.Up;
            if (key == PanDownKey) return GestureKeys.Down;
            if (key == PanLeftKey) return GestureKeys.Left;
            if (key == PanRightKey) return GestureKeys.Right;
            if (key == ZoomInKey) return GestureKeys.ZoomIn;
            if (key == ZoomOutKey) return GestureKeys.ZoomOut;

            return null;
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
