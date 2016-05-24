using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {

    /// <summary>
    /// A Singleton for managing a number of asynchronous operations
    /// </summary>
    public sealed class AsyncManager : Singleton<AsyncManager> {

        // Set of all asynchronous operations managed by the manager
        private readonly List<AsyncOperation> _operations = new List<AsyncOperation>();
        private static readonly List<Action> _waitingSynchronousActions = new List<Action>(); 

        /// <summary>
        /// The overall progress of all of the asynchronous actions. Shown as a ratio in the range [0.0, 1.0]
        /// </summary>
        public float Progress {
            get { return OperationsInProgress > 0 ? 1.0f : _operations.Average(op => op.progress); }
        }

        /// <summary>
        /// The number of operations in progress currently
        /// </summary>
        public int OperationsInProgress {
            get { return _operations.Count; }
        }

        /// <summary>
        /// Add a async operation to manage.
        /// Can optionally provide a callback to be called once the operation is finished.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="operation"/> is null</exception>
        /// <param name="operation">the operation to manage</param>
        /// <param name="callback">optional parameter, if not null, will be called after finish executing</param>
        public void AddOperation(AsyncOperation operation, Action callback = null) {
            Check.NotNull("operation", operation);
            _operations.Add(operation);
            StartCoroutine(WaitForOperation(operation, callback));
        }

        /// <summary>
        /// Adds a resource request to manage.
        /// Can optionally provide a callback to be called once the operation is finished.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="request"/> is null</exception>
        /// <typeparam name="T">the type of object loaded by</typeparam>
        /// <param name="request">the ResourceRequest to manage</param>
        /// <param name="callback">optional parameter, if not null, will be called after finish executing</param>
        public void AddOpreation<T>(ResourceRequest request, Action<T> callback = null) where T : Object {
            Check.NotNull("request", request);
            _operations.Add(request);
            StartCoroutine(WaitForResource(request, callback));
        }

        public static void AddSynchronousAction(Action action) {
            if(action != null)
                _waitingSynchronousActions.Add(action);
        }

        protected override void Awake() {
            base.Awake();
            Flush();
        }

        void Start() {
            Flush();
        }

        void Update() {
            Flush();
        }

        static void Flush() {
            if (_waitingSynchronousActions.Count <= 0)
                return;
            foreach (Action action in _waitingSynchronousActions)
                action();
            _waitingSynchronousActions.Clear();
        }

        IEnumerator WaitForOperation(AsyncOperation operation, Action callback) {
            yield return operation;
            _operations.Remove(operation);
            if (callback != null)
                callback();
        }

        IEnumerator WaitForResource<T>(ResourceRequest request, Action<T> callback) where T : Object {
            yield return request;
            _operations.Remove(request);
            if (callback == null)
                yield break;
            var obj = request.asset as T;
            callback(obj);
        }
    }
}
