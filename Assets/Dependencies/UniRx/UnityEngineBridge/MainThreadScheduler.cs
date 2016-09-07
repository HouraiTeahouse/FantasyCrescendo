using System;
using System.Collections;
using UnityEngine;

namespace UniRx {

#if UniRxLibrary
    public static partial class SchedulerUnity
    {
#else
    public static partial class Scheduler {

        class EndOfFrameMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing {

            public EndOfFrameMainThreadScheduler() { MainThreadDispatcher.Initialize(); }

            public DateTimeOffset Now {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action) { return Schedule(TimeSpan.Zero, action); }

            public IDisposable Schedule(TimeSpan dueTime, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(dueTime);

                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(DelayAction(time, action, d));

                return d;
            }

            public IDisposable SchedulePeriodic(TimeSpan period, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(period);

                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(PeriodicAction(time, action, d));

                return d;
            }

            public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action) {
                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(ImmediateAction(state, action, cancel));
            }

            IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation) {
                yield return null;
                if (cancellation.IsDisposed)
                    yield break;

                MainThreadDispatcher.UnsafeSend(action, state);
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation) {
                if (dueTime == TimeSpan.Zero) {
                    yield return null;
                    if (cancellation.IsDisposed)
                        yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else {
                    var elapsed = 0f;
                    var dt = (float) dueTime.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        elapsed += Time.deltaTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    }
                }
            }

            IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation) {
                // zero == every frame
                if (period == TimeSpan.Zero) {
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            yield break;

                        MainThreadDispatcher.UnsafeSend(action);
                    }
                }
                else {
                    var elapsed = 0f;
                    var dt = (float) period.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        elapsed += Time.deltaTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            elapsed = 0;
                        }
                    }
                }
            }

            public IDisposable Schedule(DateTimeOffset dueTime, Action action) {
                return Schedule(dueTime - Now, action);
            }

        }

        class FixedUpdateMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing {

            public FixedUpdateMainThreadScheduler() { MainThreadDispatcher.Initialize(); }

            public DateTimeOffset Now {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action) { return Schedule(TimeSpan.Zero, action); }

            public IDisposable Schedule(TimeSpan dueTime, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(dueTime);

                MainThreadDispatcher.StartFixedUpdateMicroCoroutine(DelayAction(time, action, d));

                return d;
            }

            public IDisposable SchedulePeriodic(TimeSpan period, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(period);

                MainThreadDispatcher.StartFixedUpdateMicroCoroutine(PeriodicAction(time, action, d));

                return d;
            }

            public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action) {
                MainThreadDispatcher.StartFixedUpdateMicroCoroutine(ImmediateAction(state, action, cancel));
            }

            IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation) {
                yield return null;
                if (cancellation.IsDisposed)
                    yield break;

                MainThreadDispatcher.UnsafeSend(action, state);
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation) {
                if (dueTime == TimeSpan.Zero) {
                    yield return null;
                    if (cancellation.IsDisposed)
                        yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else {
                    float startTime = Time.fixedTime;
                    var dt = (float) dueTime.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        float elapsed = Time.fixedTime - startTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    }
                }
            }

            IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation) {
                // zero == every frame
                if (period == TimeSpan.Zero) {
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            yield break;

                        MainThreadDispatcher.UnsafeSend(action);
                    }
                }
                else {
                    float startTime = Time.fixedTime;
                    var dt = (float) period.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        float ft = Time.fixedTime;
                        float elapsed = ft - startTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            startTime = ft;
                        }
                    }
                }
            }

            public IDisposable Schedule(DateTimeOffset dueTime, Action action) {
                return Schedule(dueTime - Now, action);
            }

        }

        class IgnoreTimeScaleMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing {

            static class QueuedAction<T> {

                public static readonly Action<object> Instance = new Action<object>(Invoke);

                public static void Invoke(object state) {
                    var t = (Tuple<ICancelable, T, Action<T>>) state;

                    if (!t.Item1.IsDisposed) {
                        t.Item3(t.Item2);
                    }
                }

            }

            readonly Action<object> scheduleAction;

            public IgnoreTimeScaleMainThreadScheduler() {
                MainThreadDispatcher.Initialize();
                scheduleAction = new Action<object>(Schedule);
            }

            public DateTimeOffset Now {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action) {
                var d = new BooleanDisposable();
                MainThreadDispatcher.Post(scheduleAction, Tuple.Create(d, action));
                return d;
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(dueTime);

                MainThreadDispatcher.SendStartCoroutine(DelayAction(time, action, d));

                return d;
            }

            public IDisposable SchedulePeriodic(TimeSpan period, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(period);

                MainThreadDispatcher.SendStartCoroutine(PeriodicAction(time, action, d));

                return d;
            }

            public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action) {
                MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
            }

            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation) {
                if (dueTime == TimeSpan.Zero) {
                    yield return null;
                    if (cancellation.IsDisposed)
                        yield break;

                    MainThreadDispatcher.UnsafeSend(action);
                }
                else {
                    var elapsed = 0f;
                    var dt = (float) dueTime.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        elapsed += Time.unscaledDeltaTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            break;
                        }
                    }
                }
            }

            IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation) {
                // zero == every frame
                if (period == TimeSpan.Zero) {
                    while (true) {
                        yield return null; // not immediately, run next frame
                        if (cancellation.IsDisposed)
                            yield break;

                        MainThreadDispatcher.UnsafeSend(action);
                    }
                }
                else {
                    var elapsed = 0f;
                    var dt = (float) period.TotalSeconds;
                    while (true) {
                        yield return null;
                        if (cancellation.IsDisposed)
                            break;

                        elapsed += Time.unscaledDeltaTime;
                        if (elapsed >= dt) {
                            MainThreadDispatcher.UnsafeSend(action);
                            elapsed = 0;
                        }
                    }
                }
            }

            void Schedule(object state) {
                var t = (Tuple<BooleanDisposable, Action>) state;
                if (!t.Item1.IsDisposed) {
                    t.Item2();
                }
            }

            public IDisposable Schedule(DateTimeOffset dueTime, Action action) {
                return Schedule(dueTime - Now, action);
            }

        }

        class MainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing {

            static class QueuedAction<T> {

                public static readonly Action<object> Instance = new Action<object>(Invoke);

                public static void Invoke(object state) {
                    var t = (Tuple<ICancelable, T, Action<T>>) state;

                    if (!t.Item1.IsDisposed) {
                        t.Item3(t.Item2);
                    }
                }

            }

            readonly Action<object> scheduleAction;

            public MainThreadScheduler() {
                MainThreadDispatcher.Initialize();
                scheduleAction = new Action<object>(Schedule);
            }

            public DateTimeOffset Now {
                get { return Scheduler.Now; }
            }

            public IDisposable Schedule(Action action) {
                var d = new BooleanDisposable();
                MainThreadDispatcher.Post(scheduleAction, Tuple.Create(d, action));
                return d;
            }

            public IDisposable Schedule(TimeSpan dueTime, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(dueTime);

                MainThreadDispatcher.SendStartCoroutine(DelayAction(time, action, d));

                return d;
            }

            public IDisposable SchedulePeriodic(TimeSpan period, Action action) {
                var d = new BooleanDisposable();
                TimeSpan time = Normalize(period);

                MainThreadDispatcher.SendStartCoroutine(PeriodicAction(time, action, d));

                return d;
            }

            public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action) {
                MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
            }

            // delay action is run in StartCoroutine
            // Okay to action run synchronous and guaranteed run on MainThread
            IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation) {
                // zero == every frame
                if (dueTime == TimeSpan.Zero) {
                    yield return null; // not immediately, run next frame
                }
                else {
                    yield return new WaitForSeconds((float) dueTime.TotalSeconds);
                }

                if (cancellation.IsDisposed)
                    yield break;
                MainThreadDispatcher.UnsafeSend(action);
            }

            IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation) {
                // zero == every frame
                if (period == TimeSpan.Zero) {
                    while (true) {
                        yield return null; // not immediately, run next frame
                        if (cancellation.IsDisposed)
                            yield break;

                        MainThreadDispatcher.UnsafeSend(action);
                    }
                }
                else {
                    var seconds = (float) (period.TotalMilliseconds / 1000.0);
                    var yieldInstruction = new WaitForSeconds(seconds); // cache single instruction object

                    while (true) {
                        yield return yieldInstruction;
                        if (cancellation.IsDisposed)
                            yield break;

                        MainThreadDispatcher.UnsafeSend(action);
                    }
                }
            }

            void Schedule(object state) {
                var t = (Tuple<BooleanDisposable, Action>) state;
                if (!t.Item1.IsDisposed) {
                    t.Item2();
                }
            }

            public IDisposable Schedule(DateTimeOffset dueTime, Action action) {
                return Schedule(dueTime - Now, action);
            }

            void ScheduleQueueing<T>(object state) {
                var t = (Tuple<ICancelable, T, Action<T>>) state;
                if (!t.Item1.IsDisposed) {
                    t.Item3(t.Item2);
                }
            }

        }

        static IScheduler mainThread;

        static IScheduler mainThreadIgnoreTimeScale;

        static IScheduler mainThreadFixedUpdate;

        static IScheduler mainThreadEndOfFrame;

        /// <summary> Unity native MainThread Queue Scheduler. Run on mainthread and delayed on coroutine update loop, elapsed time
        /// is calculated based on Time.time. </summary>
        public static IScheduler MainThread {
            get { return mainThread ?? (mainThread = new MainThreadScheduler()); }
        }

        /// <summary> Another MainThread scheduler, delay elapsed time is calculated based on Time.unscaledDeltaTime. </summary>
        public static IScheduler MainThreadIgnoreTimeScale {
            get {
                return mainThreadIgnoreTimeScale
                    ?? (mainThreadIgnoreTimeScale = new IgnoreTimeScaleMainThreadScheduler());
            }
        }

        /// <summary> Run on fixed update mainthread, delay elapsed time is calculated based on Time.fixedTime. </summary>
        public static IScheduler MainThreadFixedUpdate {
            get { return mainThreadFixedUpdate ?? (mainThreadFixedUpdate = new FixedUpdateMainThreadScheduler()); }
        }

        /// <summary> Run on end of frame mainthread, delay elapsed time is calculated based on Time.deltaTime. </summary>
        public static IScheduler MainThreadEndOfFrame {
            get { return mainThreadEndOfFrame ?? (mainThreadEndOfFrame = new EndOfFrameMainThreadScheduler()); }
        }

        public static void SetDefaultForUnity() {
            DefaultSchedulers.ConstantTimeOperations = Immediate;
            DefaultSchedulers.TailRecursion = Immediate;
            DefaultSchedulers.Iteration = CurrentThread;
            DefaultSchedulers.TimeBasedOperations = MainThread;
            DefaultSchedulers.AsyncConversions = ThreadPool;
        }
#endif
    }

}