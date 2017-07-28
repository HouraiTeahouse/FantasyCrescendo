using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {

    public static class TaskExtensions {

        public static ITask<T> Then<T>(this ITask task, Func<T> func) {
            return task.Then(() => Task.FromResult(func()));
        }

        #region ThenAll Overloads
        public static ITask ThenAll(this ITask task, params ITask[] set) { return task.ThenAll(set as IEnumerable<ITask>); }
        public static ITask<T[]> ThenAll<T>(this ITask task, params ITask<T>[] set) { return task.ThenAll(set as IEnumerable<ITask<T>>); }

        public static ITask ThenAll(this ITask task, 
                                    IEnumerable<ITask> set) {
            return Argument.NotNull(task).Then(() => Task.All(set));
        }

        public static ITask ThenAll(this ITask task, 
                                    Func<IEnumerable<ITask>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(() => Task.All(set()));
        }

        public static ITask ThenAll<T>(this ITask<T> task, 
                                       Func<T, IEnumerable<ITask>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(t => Task.All(set(t)));
        }

        public static ITask<T[]> ThenAll<T>(this ITask task, 
                                            IEnumerable<ITask<T>> set) {
            return Argument.NotNull(task).Then(() => Task.All(set));
        }

        public static ITask<T[]> ThenAll<T>(this ITask task, 
                                            Func<IEnumerable<ITask<T>>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(() => Task.All(set()));
        }

        public static ITask<TResult[]> ThenAll<T, TResult>(this ITask<T> task, 
                                                           Func<T, IEnumerable<ITask<TResult>>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(t => Task.All(set(t)));
        }
#endregion

#region ThenAny Overlaods
        public static ITask ThenAny(this ITask task, params ITask[] set) { return task.ThenAny(set as IEnumerable<ITask>); }
        public static ITask<T> ThenAny<T>(this ITask task, params ITask<T>[] set) { return task.ThenAny(set as IEnumerable<ITask<T>>); }
        public static ITask ThenAny(this ITask task, 
                                    IEnumerable<ITask> set) {
            return Argument.NotNull(task).Then(() => Task.Any(set));
        }

        public static ITask<T> ThenAny<T>(this ITask task, 
                                          IEnumerable<ITask<T>> set) {
            return Argument.NotNull(task).Then(() => Task.Any(set));
        }

        public static ITask ThenAny(this ITask task, 
                                    Func<IEnumerable<ITask>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(() => Task.Any(set()));
        }

        public static ITask<T> ThenAny<T>(this ITask task, 
                                          Func<IEnumerable<ITask<T>>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(() => Task.Any(set()));
        }

        public static ITask<TResult> ThenAny<T, TResult>(this ITask<T> task, 
                                                         Func<T, IEnumerable<ITask<TResult>>> set) {
            Argument.NotNull(set);
            return Argument.NotNull(task).Then(t => Task.Any(set(t)));
        }
#endregion

    }

    public interface ITask : IResolvable, IRejectable {

        TaskState State { get; }
        ITask Then(Action onResolved);
        ITask Then(Func<ITask> onResolved);
        ITask<T> Then<T>(Func<ITask<T>> onResolved);
        ITask Catch(Action<Exception> onError);
        void Done();

    }

    public interface ITask<T> : ITask, IResolvable<T> {

        ITask Then(Action<T> onResolve);
        ITask<TResult> Then<TResult>(Func<T, TResult> onResolve);
        ITask Then(Func<T, ITask> onResolve);
        ITask<TResult> Then<TResult>(Func<T, ITask<TResult>> onResolve);
        new ITask<T> Catch(Action<Exception> onError);

    }

    public interface ITaskInfo : IUniqueEntity<int>, INameable {
    }

    public enum TaskState {
        Pending,
        Error,
        Success
    }

    public class Task : ITask, ITaskInfo {

        class ResolveHandler {

            public readonly IRejectable Rejectable;
            public readonly Action Callback;

            public ResolveHandler(IRejectable rejectable, Action callback) {
                Rejectable = rejectable;
                Callback = callback;
            }

        }

        class RejectHandler {

            public readonly IRejectable Rejectable;
            public readonly Action<Exception> Callback;

            public RejectHandler(IRejectable rejectable, Action<Exception> callback) {
                Rejectable = rejectable;
                Callback = callback;
            }

        }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public TaskState State { get; protected set; }
        public Exception Exception { get; private set; }

        public static IEnumerable<ITaskInfo> PendingTasks {
            get { return pendingTasks; }
        }

        static Task() {
            EnableTracking = false;
            pendingTasks = new HashSet<ITaskInfo>();
        }

        // Whether or not to track task use
        public static bool EnableTracking;

        public static event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        static readonly HashSet<ITaskInfo> pendingTasks;

        List<ResolveHandler> _resolveHandlers;
        List<RejectHandler> _rejectHandlers;

        protected internal static int NextTaskId;

        public Task(string name = null) {
            State = TaskState.Pending;
            ID = ++NextTaskId;
            Name = name;
            if (EnableTracking) {
                pendingTasks.Add(this);
            }
        }

        public Task(Action<Action, Action<Exception>> resolver) : this() {
            Argument.NotNull(resolver);
            try {
                resolver(Resolve, Reject);
            } catch(Exception ex) {
                Reject(ex);
            }
        }

        public void Resolve() {
            if(State != TaskState.Pending)
                throw new InvalidOperationException(
                    string.Format("Attempted to resolved a task that is already in state {0}. "
                        + "A task can only be resolved when it is still in state: {1}", 
                        State, TaskState.Pending));
            State = TaskState.Success;
            pendingTasks.Remove(this);
            OnResolve();
        }

        public void Reject(Exception error) {
            if(State != TaskState.Pending)
                throw new InvalidOperationException(
                    string.Format("Attempted to reject a task that is already in state {0}. "
                        + "A task can only be rejected when it is still in state: {1}", 
                        State, TaskState.Pending));
            Argument.NotNull(error);
            State = TaskState.Error;
            Exception = error;
            pendingTasks.Remove(this);
            OnReject(error);
        }

        void OnResolve() {
            if(_resolveHandlers != null)
                foreach (ResolveHandler handler in _resolveHandlers.ToArray())
                    InvokeResolve(handler.Callback, handler.Rejectable);
            Clear();
        }

        void OnReject(Exception error) {
            if(_rejectHandlers != null && _rejectHandlers.Any())
                foreach (RejectHandler handler in _rejectHandlers.ToArray())
                    InvokeReject(handler.Callback, error, handler.Rejectable);
            Clear();
        }

        void Clear() {
            _resolveHandlers = null;
            _rejectHandlers = null;
        }

        public ITask Then(Action onResolved) {
            var task = new Task();
            ActionHandlers(task,
                () => {
                    // Avoiding use of SafeInvoke here due higher chances of stack overflow
                    if (onResolved != null)
                        onResolved();
                    task.Resolve();
                });
            return task;
        }

        public ITask Then(Func<ITask> onResolved) {
            var task = new Task();
            ActionHandlers(task, () => onResolved().Then(() => task.Resolve()).Catch(task.Reject));
            return task;
        }

        public ITask<T> Then<T>(Func<ITask<T>> onResolved) {
            var task = new Task<T>();
            ActionHandlers(task,() => onResolved().Then(t => task.Resolve(t)).Catch(task.Reject));
            return task;
        }

        public ITask Catch(Action<Exception> onError) {
            var task = new Task();
            ActionHandlers(task, task.Resolve, ex => {
                onError.SafeInvoke(ex);
                task.Reject(ex);
            });
            return task;
        }

        public void Done() {
            Catch(ex => {
                if(UnhandledException != null)
                    UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
            });
        }

        void InvokeResolve(Action callback, IRejectable rejectable) {
            Argument.NotNull(rejectable);
            try {
                callback.SafeInvoke();
            } catch(Exception ex) {
                rejectable.Reject(ex);
            }
        }

        void AddResolveHandler(Action onError, IRejectable rejectable) {
            Argument.NotNull(onError);
            Argument.NotNull(rejectable);
            if (_resolveHandlers == null)
                _resolveHandlers = new List<ResolveHandler>();
            _resolveHandlers.Add(new ResolveHandler(rejectable, onError));
        }

        protected void InvokeReject(Action<Exception> callback, Exception exception, IRejectable rejectable) {
            Argument.NotNull(rejectable);
            try {
                callback.SafeInvoke(exception);
            } catch(Exception ex) {
                if(rejectable != this)
                    rejectable.Reject(ex);
            }
        }

        protected void AddRejectHandler(Action<Exception> onError, IRejectable rejectable) {
            Argument.NotNull(onError);
            Argument.NotNull(rejectable);
            if (_rejectHandlers == null)
                _rejectHandlers = new List<RejectHandler>();
            _rejectHandlers.Add(new RejectHandler(rejectable, onError));
        }

        protected void ActionHandlers(ITask task, Action resolveHandler = null, Action<Exception> rejecthandler = null) {
            if (resolveHandler == null)
                resolveHandler = task.Resolve;
            if (rejecthandler == null)
                rejecthandler = task.Reject;
            if (State == TaskState.Success)
                InvokeResolve(resolveHandler, task);
            else if(State == TaskState.Error)
                InvokeReject(rejecthandler, Exception, this);
            else {
                AddResolveHandler(resolveHandler, task);
                AddRejectHandler(rejecthandler, this);
            }
        }

        public static ITask Resolved {
            get { return new Task {State = TaskState.Success}; }
        }

        public static ITask<T> FromResult<T>(T result) {
            var task = new Task<T>();
            task.Resolve(result);
            return task;
        }

        public static IEnumerable<ITask<T>> FromResults<T>(IEnumerable<T> results) {
            return results.EmptyIfNull().Select(FromResult);
        }

        public static IEnumerable<ITask<T>> FromResults<T>(params T[] results) {
            return FromResults(results as IEnumerable<T>);
        }

        public static ITask FromError(Exception error) {
            var task = new Task();
            task.Reject(error);
            return task;
        }

        public static ITask<T> FromError<T>(Exception error) {
            var task = new Task<T>();
            task.Reject(error);
            return task;
        }

        public static ITask Any(params ITask[] tasks) { return Any(tasks as IEnumerable<ITask>); }
        public static ITask<T> Any<T>(params ITask<T>[] tasks) { return Any(tasks as IEnumerable<ITask<T>>); }

        public static ITask Any(IEnumerable<ITask> tasks) {
            ITask[] allTasks = tasks.IgnoreNulls().ToArray();
            int count = allTasks.Length;
            if (count <= 0)
                return Resolved;
            var task = new Task();
            foreach (ITask subTask in allTasks) {
                subTask.Catch(ex => {
                    if (task.State == TaskState.Pending)
                        task.Reject(ex);
                }).Then(new Action(task.Resolve));
            }
            return task;
        }

        public static ITask<T> Any<T>(IEnumerable<ITask<T>> tasks) {
            ITask<T>[] allTasks = tasks.IgnoreNulls().ToArray();
            int count = allTasks.Length;
            if (count <= 0)
                return FromResult(default(T));
            ITask<T> task = new Task<T>();
            foreach (ITask<T> subTask in allTasks) {
                subTask.Catch(ex => {
                    if (task.State == TaskState.Pending)
                        task.Reject(ex);
                }).Then(new Action<T>(task.Resolve));
            }
            return task;
        }

        public static ITask All(params ITask[] tasks) { return All(tasks as IEnumerable<ITask>); }
        public static ITask<T[]> All<T>(params ITask<T>[] tasks) { return All(tasks as IEnumerable<ITask<T>>); }

        static void Each<T>(IEnumerable<T> enumeration, Action<T, int> fn) {
            var index = 0;
            foreach (T obj in enumeration.EmptyIfNull()) {
                fn(obj, index);
                index++;
            }
        }

        public static ITask All(IEnumerable<ITask> tasks) {
            ITask[] allTasks = tasks.IgnoreNulls().ToArray();
            int count = allTasks.Length;
            if (count <= 0)
                return Resolved;
            var task = new Task();
            Each(allTasks,
                (subtask, i) => {
                    subtask.Catch(ex => {
                        if (task.State == TaskState.Pending)
                            task.Reject(ex);
                    }).Then(() => {
                        --count;
                        if (count <= 0)
                            task.Resolve();
                    });
                });
            return task;
        }

        public static ITask<T[]> All<T>(IEnumerable<ITask<T>> tasks) {
            ITask<T>[] allTasks = tasks.ToArray();
            int count = allTasks.Length;
            if (count <= 0)
                return FromResult(new T[0]);
            var results = new T[allTasks.Length];
            ITask<T[]> task = new Task<T[]>();
            Each(allTasks, (subtask, i) => {
                if (subtask == null)
                    results[i] = default(T);
                else {
                    subtask.Catch(ex => {
                        if (task.State == TaskState.Pending)
                            task.Reject(ex);
                    }).Then(t => {
                        results[i] = t;
                        count--;
                        if (count <= 0)
                            task.Resolve(results);
                    });
                }
            });
            return task;
        }

        public static ITask Sequence(params Func<ITask>[] funcs) { return Sequence(funcs as IEnumerable<Func<ITask>>); }
        public static ITask Sequence(IEnumerable<Func<ITask>> funcs) {
            return funcs.Aggregate(Resolved, (prevTask, fn) => prevTask.Then(fn));
        }

    }

    public class Task<T> : Task, ITask<T> {

        class ResolveHandler {

            public readonly IRejectable Rejectable;
            public readonly Action<T> Callback;

            public ResolveHandler(IRejectable rejectable, Action<T> callback) {
                Rejectable = rejectable;
                Callback = callback;
            }

        }

        List<ResolveHandler> _resolveHandlers;

        public Task() {
        }

        public Task(Action<Action<T>, Action<Exception>> actionHandlers) {
            Argument.NotNull(actionHandlers);
            try {
                actionHandlers(Resolve, Reject);
            } catch(Exception ex) {
                Reject(ex);
            }
        }

        public T Result { get; private set; }

        public void Resolve(T value) {
            Resolve();
            Result = value;
            if(_resolveHandlers != null)
                foreach (ResolveHandler handler in _resolveHandlers.ToArray())
                    InvokeResolve(handler.Callback, handler.Rejectable);
        }

        public ITask Then(Action<T> onResolve) {
            var task = new Task();
            ActionHandlers(task,
                t => {
                    onResolve(Result);
                    task.Resolve();
                });
            return task;
        }

        public ITask<TResult> Then<TResult>(Func<T, TResult> onResolve) {
            Argument.NotNull(onResolve);
            return Then(t => FromResult(onResolve(t)));
        }

        public ITask Then(Func<T, ITask> onResolve) {
            var task = new Task();
            ActionHandlers(task,
                t => {
                    if (onResolve != null)
                        onResolve(Result).Then(new Action(task.Resolve)).Catch(task.Reject);
                    else
                        task.Resolve();
                });
            return task;
        }

        public ITask<TResult> Then<TResult>(Func<T, ITask<TResult>> onResolve) {
            Argument.NotNull(onResolve);
            var task = new Task<TResult>();
            ActionHandlers(task, t => onResolve(t).Then(new Action<TResult>(task.Resolve)));
            return task;
        }

        void InvokeResolve(Action<T> callback, IRejectable rejectable) {
            Argument.NotNull(rejectable);
            try {
                callback.SafeInvoke(Result);
            } catch(Exception ex) {
                rejectable.Reject(ex);
            }
        }

        void AddResolveHandler(Action<T> onResolve, IRejectable rejectable) {
            Argument.NotNull(onResolve);
            Argument.NotNull(rejectable);
            if (_resolveHandlers == null)
                _resolveHandlers = new List<ResolveHandler>();
            _resolveHandlers.Add(new ResolveHandler(rejectable, onResolve));
        }

        protected void ActionHandlers(ITask task, Action<T> resolveHandler, Action<Exception> rejectHandler = null) {
            if (rejectHandler == null)
                rejectHandler = task.Reject;
            if (State == TaskState.Success) 
                InvokeResolve(resolveHandler, task);
            else if(State == TaskState.Error)
                InvokeReject(rejectHandler, Exception, this);
            else {
                AddResolveHandler(resolveHandler, task);
                AddRejectHandler(rejectHandler, this);
            }
        }

        public new ITask<T> Catch(Action<Exception> onError) {
            var task = new Task<T>();
            ActionHandlers(task, task.Resolve,
                ex => {
                    onError.SafeInvoke(ex);
                    task.Reject(ex);
                });
            return task;
        }

    }

}
