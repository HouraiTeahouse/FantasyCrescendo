using System;
using UniRx.InternalUtil;
using UnityEngine.UI;

namespace UniRx {

    public interface IReactiveCommand<T> : IObservable<T> {

        IReadOnlyReactiveProperty<bool> CanExecute { get; }
        bool Execute(T parameter);

    }

    public interface IAsyncReactiveCommand<T> {

        IReadOnlyReactiveProperty<bool> CanExecute { get; }
        IDisposable Execute(T parameter);
        IDisposable Subscribe(Func<T, IObservable<Unit>> asyncAction);

    }

    /// <summary> Represents ReactiveCommand&lt;Unit&gt; </summary>
    public class ReactiveCommand : ReactiveCommand<Unit> {

        /// <summary> CanExecute is always true. </summary>
        public ReactiveCommand() : base() {
        }

        /// <summary> CanExecute is changed from canExecute sequence. </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true)
            : base(canExecuteSource, initialValue) {
        }

        /// <summary> Push null to subscribers. </summary>
        public bool Execute() {
            return Execute(Unit.Default);
        }

        /// <summary> Force push parameter to subscribers. </summary>
        public void ForceExecute() {
            ForceExecute(Unit.Default);
        }

    }

    public class ReactiveCommand<T> : IReactiveCommand<T>, IDisposable {

        readonly Subject<T> trigger = new Subject<T>();
        readonly IDisposable canExecuteSubscription;

        ReactiveProperty<bool> canExecute;

        /// <summary> CanExecute is always true. </summary>
        public ReactiveCommand() {
            canExecute = new ReactiveProperty<bool>(true);
            canExecuteSubscription = Disposable.Empty;
        }

        /// <summary> CanExecute is changed from canExecute sequence. </summary>
        public ReactiveCommand(IObservable<bool> canExecuteSource, bool initialValue = true) {
            canExecute = new ReactiveProperty<bool>(initialValue);
            canExecuteSubscription = canExecuteSource.DistinctUntilChanged()
                .SubscribeWithState(canExecute, (b, c) => c.Value = b);
        }

        public bool IsDisposed { get; private set; }

        /// <summary> Stop all subscription and lock CanExecute is false. </summary>
        public void Dispose() {
            if (IsDisposed)
                return;

            IsDisposed = true;
            canExecute.Dispose();
            trigger.OnCompleted();
            trigger.Dispose();
            canExecuteSubscription.Dispose();
        }

        public IReadOnlyReactiveProperty<bool> CanExecute {
            get { return canExecute; }
        }

        /// <summary> Push parameter to subscribers when CanExecute. </summary>
        public bool Execute(T parameter) {
            if (canExecute.Value) {
                trigger.OnNext(parameter);
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary> Subscribe execute. </summary>
        public IDisposable Subscribe(IObserver<T> observer) {
            return trigger.Subscribe(observer);
        }

        /// <summary> Force push parameter to subscribers. </summary>
        public void ForceExecute(T parameter) {
            trigger.OnNext(parameter);
        }

    }

    /// <summary> Variation of ReactiveCommand, when executing command then CanExecute = false after CanExecute = true. </summary>
    public class AsyncReactiveCommand : AsyncReactiveCommand<Unit> {

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. </summary>
        public AsyncReactiveCommand() : base() {
        }

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource) : base(canExecuteSource) {
        }

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. The source is shared
        /// between other AsyncReactiveCommand. </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute) : base(sharedCanExecute) {
        }

        public IDisposable Execute() { return base.Execute(Unit.Default); }

    }

    /// <summary> Variation of ReactiveCommand, canExecute is changed when executing command then CanExecute = false after
    /// CanExecute = true. </summary>
    public class AsyncReactiveCommand<T> : IAsyncReactiveCommand<T> {

        class Subscription : IDisposable {

            readonly AsyncReactiveCommand<T> parent;
            readonly Func<T, IObservable<Unit>> asyncAction;

            public Subscription(AsyncReactiveCommand<T> parent, Func<T, IObservable<Unit>> asyncAction) {
                this.parent = parent;
                this.asyncAction = asyncAction;
            }

            public void Dispose() {
                lock (parent.gate) {
                    parent.asyncActions = parent.asyncActions.Remove(asyncAction);
                }
            }

        }

        ImmutableList<Func<T, IObservable<Unit>>> asyncActions = ImmutableList<Func<T, IObservable<Unit>>>.Empty;

        readonly object gate = new object();
        readonly IReactiveProperty<bool> canExecuteSource;
        readonly IReadOnlyReactiveProperty<bool> canExecute;

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. </summary>
        public AsyncReactiveCommand() {
            canExecuteSource = new ReactiveProperty<bool>(true);
            canExecute = canExecuteSource;
        }

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. </summary>
        public AsyncReactiveCommand(IObservable<bool> canExecuteSource) {
            this.canExecuteSource = new ReactiveProperty<bool>(true);
            canExecute = canExecute.CombineLatest(canExecuteSource, (x, y) => x && y).ToReactiveProperty();
        }

        /// <summary> CanExecute is automatically changed when executing to false and finished to true. The source is shared
        /// between other AsyncReactiveCommand. </summary>
        public AsyncReactiveCommand(IReactiveProperty<bool> sharedCanExecute) {
            canExecuteSource = sharedCanExecute;
            canExecute = sharedCanExecute;
        }

        public bool IsDisposed { get; private set; }

        public IReadOnlyReactiveProperty<bool> CanExecute {
            get { return canExecute; }
        }

        /// <summary> Push parameter to subscribers when CanExecute. </summary>
        public IDisposable Execute(T parameter) {
            if (canExecute.Value) {
                canExecuteSource.Value = false;
                Func<T, IObservable<Unit>>[] a = asyncActions.Data;
                if (a.Length == 1) {
                    try {
                        IObservable<Unit> asyncState = a[0].Invoke(parameter) ?? Observable.ReturnUnit();
                        return asyncState.Finally(() => canExecuteSource.Value = true).Subscribe();
                    } catch {
                        canExecuteSource.Value = true;
                        throw;
                    }
                }
                else {
                    var xs = new IObservable<Unit>[a.Length];
                    try {
                        for (var i = 0; i < a.Length; i++) {
                            xs[i] = a[i].Invoke(parameter) ?? Observable.ReturnUnit();
                        }
                    } catch {
                        canExecuteSource.Value = true;
                        throw;
                    }

                    return Observable.WhenAll(xs).Finally(() => canExecuteSource.Value = true).Subscribe();
                }
            }
            else {
                return Disposable.Empty;
            }
        }

        /// <summary> Subscribe execute. </summary>
        public IDisposable Subscribe(Func<T, IObservable<Unit>> asyncAction) {
            lock (gate) {
                asyncActions = asyncActions.Add(asyncAction);
            }

            return new Subscription(this, asyncAction);
        }

    }

    public static class ReactiveCommandExtensions {

        /// <summary> Create non parameter commands. CanExecute is changed from canExecute sequence. </summary>
        public static ReactiveCommand ToReactiveCommand(this IObservable<bool> canExecuteSource,
                                                        bool initialValue = true) {
            return new ReactiveCommand(canExecuteSource, initialValue);
        }

        /// <summary> Create parametered comamnds. CanExecute is changed from canExecute sequence. </summary>
        public static ReactiveCommand<T> ToReactiveCommand<T>(this IObservable<bool> canExecuteSource,
                                                              bool initialValue = true) {
            return new ReactiveCommand<T>(canExecuteSource, initialValue);
        }

#if !UniRxLibrary

        // for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        /// <summary> Bind RaectiveCommand to button's interactable and onClick. </summary>
        public static IDisposable BindTo(this ReactiveCommand<Unit> command, Button button) {
            IDisposable d1 = command.CanExecute.SubscribeToInteractable(button);
            IDisposable d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            return StableCompositeDisposable.Create(d1, d2);
        }

        /// <summary> Bind RaectiveCommand to button's interactable and onClick and register onClick action to command. </summary>
        public static IDisposable BindToOnClick(this ReactiveCommand<Unit> command, Button button, Action<Unit> onClick) {
            IDisposable d1 = command.CanExecute.SubscribeToInteractable(button);
            IDisposable d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            IDisposable d3 = command.Subscribe(onClick);

            return StableCompositeDisposable.Create(d1, d2, d3);
        }

        /// <summary> Bind canExecuteSource to button's interactable and onClick and register onClick action to command. </summary>
        public static IDisposable BindToButtonOnClick(this IObservable<bool> canExecuteSource,
                                                      Button button,
                                                      Action<Unit> onClick,
                                                      bool initialValue = true) {
            return ToReactiveCommand(canExecuteSource, initialValue).BindToOnClick(button, onClick);
        }

#endif

#endif
    }

    public static class AsyncReactiveCommandExtensions {

        public static AsyncReactiveCommand ToAsyncReactiveCommand(this IReactiveProperty<bool> sharedCanExecuteSource) {
            return new AsyncReactiveCommand(sharedCanExecuteSource);
        }

        public static AsyncReactiveCommand<T> ToAsyncReactiveCommand<T>(
            this IReactiveProperty<bool> sharedCanExecuteSource) {
            return new AsyncReactiveCommand<T>(sharedCanExecuteSource);
        }

#if !UniRxLibrary

        // for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

        /// <summary> Bind AsyncRaectiveCommand to button's interactable and onClick. </summary>
        public static IDisposable BindTo(this AsyncReactiveCommand<Unit> command, Button button) {
            IDisposable d1 = command.CanExecute.SubscribeToInteractable(button);
            IDisposable d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));

            return StableCompositeDisposable.Create(d1, d2);
        }

        /// <summary> Bind AsyncRaectiveCommand to button's interactable and onClick and register async action to command. </summary>
        public static IDisposable BindToOnClick(this AsyncReactiveCommand<Unit> command,
                                                Button button,
                                                Func<Unit, IObservable<Unit>> asyncOnClick) {
            IDisposable d1 = command.CanExecute.SubscribeToInteractable(button);
            IDisposable d2 = button.OnClickAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
            IDisposable d3 = command.Subscribe(asyncOnClick);

            return StableCompositeDisposable.Create(d1, d2, d3);
        }

        /// <summary> Create AsyncReactiveCommand and bind to button's interactable and onClick and register async action to
        /// command. </summary>
        public static IDisposable BindToOnClick(this Button button, Func<Unit, IObservable<Unit>> asyncOnClick) {
            return new AsyncReactiveCommand().BindToOnClick(button, asyncOnClick);
        }

        /// <summary> Create AsyncReactiveCommand and bind sharedCanExecuteSource source to button's interactable and onClick and
        /// register async action to command. </summary>
        public static IDisposable BindToOnClick(this Button button,
                                                IReactiveProperty<bool> sharedCanExecuteSource,
                                                Func<Unit, IObservable<Unit>> asyncOnClick) {
            return sharedCanExecuteSource.ToAsyncReactiveCommand().BindToOnClick(button, asyncOnClick);
        }
#endif

#endif
    }

}