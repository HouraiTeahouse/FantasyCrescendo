using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A Singleton for managing a number of asynchronous operations </summary>
    public sealed class AsyncManager : Singleton<AsyncManager> {

        // Set of all asynchronous operations managed by the manager
        readonly List<object> _operations = new List<object>();

        /// <summary> The overall progress of all of the asynchronous actions. Shown as a ratio in the range [0.0, 1.0] </summary>
        public float Progress {
            get { return OperationsInProgress > 0 ? 1.0f : _operations.OfType<AsyncOperation>().Average(op => op.progress); }
        }

        /// <summary> The number of operations in progress currently </summary>
        public int OperationsInProgress {
            get { return _operations.Count; }
        }

        static event Action WaitingSynchronousActions;

        /// <summary> Add a async operation to manage. Can optionally provide a callback to be called once the operation is
        /// finished. </summary>
        /// <param name="operation"> the operation to manage </param>
        /// <param name="resolvable"> optional parameter, if not null, will be called after finish executing </param>
        /// <exception cref="ArgumentNullException"> <paramref name="operation" /> is null </exception>
        public void AddOperation(object operation, IResolvable resolvable = null) {
            Argument.NotNull(operation);
            _operations.Add(operation);
            StartCoroutine(Wait(operation, resolvable));
        }

        public static void AddSynchronousAction(Action action) { WaitingSynchronousActions += action; }

        protected override void Awake() {
            base.Awake();
            Flush();
        }

        void Start() { Flush(); }

        void Update() { Flush(); }

        static void Flush() {
            if (WaitingSynchronousActions == null)
                return;
            WaitingSynchronousActions();
            WaitingSynchronousActions = null;
        }

        IEnumerator Wait(object operation, IResolvable task) {
            yield return operation;
            _operations.Remove(operation);
            if (task != null)
                task.Resolve();
        }

    }

}
