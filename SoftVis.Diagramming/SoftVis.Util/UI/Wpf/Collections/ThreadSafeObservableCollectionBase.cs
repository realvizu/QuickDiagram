using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace Codartis.SoftVis.Util.UI.Wpf.Collections
{
    /// <summary>
    /// Abstract base class for thread-safe observable collections.
    /// </summary>
    /// <remarks>
    /// Adapted from:
    /// https://www.codeproject.com/articles/64936/threadsafe-observableimmutable-collection
    /// </remarks>
    public abstract class ThreadSafeObservableCollectionBase : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private bool _lockObjWasTaken;
        private readonly object _lockObj;

        /// <summary>
        /// 0=unlocked, 1=locked
        /// </summary>
        private int _lock;

        private readonly LockTypeEnum _lockType;
        public LockTypeEnum LockType
        {
            get
            {
                return _lockType;
            }
        }

        protected ThreadSafeObservableCollectionBase(LockTypeEnum lockType)
        {
            _lockType = lockType;
            _lockObj = new object();
        }

        /// <summary>
        /// Returns a valid dispatcher if this is a UI thread (can be more than one UI thread so different dispatchers are possible); 
        /// null if not a UI thread.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Dispatcher GetDispatcher()
        {
            return Dispatcher.FromThread(Thread.CurrentThread);
        }

        protected void WaitForCondition(Func<bool> condition)
        {
            var dispatcher = GetDispatcher();

            if (dispatcher == null)
            {
                switch (LockType)
                {
                    case LockTypeEnum.SpinWait:
                        SpinWait.SpinUntil(condition); // spin baby... 
                        break;
                    case LockTypeEnum.Lock:
                        var isLockTaken = false;
                        Monitor.Enter(_lockObj, ref isLockTaken);
                        _lockObjWasTaken = isLockTaken;
                        break;
                }
                return;
            }

            _lockObjWasTaken = true;
            PumpWait_PumpUntil(dispatcher, condition);
        }

        protected void PumpWait_PumpUntil(Dispatcher dispatcher, Func<bool> condition)
        {
            var frame = new DispatcherFrame();
            BeginInvokePump(dispatcher, frame, condition);
            Dispatcher.PushFrame(frame);
        }

        private static void BeginInvokePump(Dispatcher dispatcher, DispatcherFrame frame, Func<bool> condition)
        {
            dispatcher.BeginInvoke
                (
                DispatcherPriority.DataBind,
                (Action)
                    (
                    () =>
                    {
                        frame.Continue = !condition();

                        if (frame.Continue)
                            BeginInvokePump(dispatcher, frame, condition);
                    }
                    )
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DoEvents()
        {
            var dispatcher = GetDispatcher();
            if (dispatcher == null)
            {
                return;
            }

            var frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.DataBind, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryLock()
        {
            switch (LockType)
            {
                case LockTypeEnum.SpinWait:
                    return Interlocked.CompareExchange(ref _lock, 1, 0) == 0;
                case LockTypeEnum.Lock:
                    return Monitor.TryEnter(_lockObj);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Lock()
        {
            switch (LockType)
            {
                case LockTypeEnum.SpinWait:
                    WaitForCondition(() => Interlocked.CompareExchange(ref _lock, 1, 0) == 0);
                    break;
                case LockTypeEnum.Lock:
                    WaitForCondition(() => Monitor.TryEnter(_lockObj));
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unlock()
        {
            switch (LockType)
            {
                case LockTypeEnum.SpinWait:
                    _lock = 0;
                    break;
                case LockTypeEnum.Lock:
                    if (_lockObjWasTaken)
                        Monitor.Exit(_lockObj);
                    _lockObjWasTaken = false;
                    break;
            }
        }

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChangedEventHandler = CollectionChanged;

            if (notifyCollectionChangedEventHandler == null)
                return;

            foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
            {
                var dispatcherObject = handler.Target as DispatcherObject;

                if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                {
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
                }
                else
                    handler(this, args);
            }
        }

        protected virtual void RaiseNotifyCollectionChanged()
        {
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RaiseNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            RaisePropertyChanged("Count");
            RaisePropertyChanged("Item[]");
            OnCollectionChanged(args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var propertyChangedEventHandler = PropertyChanged;

            if (propertyChangedEventHandler != null)
            {
                propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public enum LockTypeEnum
        {
            SpinWait,
            Lock
        }
    }
}
