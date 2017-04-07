using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.AssetBundles;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A Singleton for managing a number of asynchronous operations </summary>
    public class AsyncManager : MonoBehaviour {

        static AsyncManager _baseBehavior;
        static AsyncManager EngineHook {
            get {
                if (_baseBehavior == null) {
                    var gameObject = new GameObject("Async Manager");
                    DontDestroyOnLoad(gameObject);
                    _baseBehavior = gameObject.AddComponent<AsyncManager>();
                }
                return _baseBehavior;
            }
        }

        // Set of all asynchronous operations managed by the manager
        static readonly List<object> _operations = new List<object>();

        /// <summary> The overall progress of all of the asynchronous actions. Shown as a ratio in the range [0.0, 1.0] </summary>
        public static float Progress {
            get { return OperationsInProgress > 0 ? 1.0f : _operations.OfType<AsyncOperation>().Average(op => op.progress); }
        }

        /// <summary> The number of operations in progress currently </summary>
        public static int OperationsInProgress {
            get { return _operations.Count; }
        }

        static event Action WaitingSynchronousActions;

        /// <summary> Add a async operation to manage. Can optionally provide a callback to be called once the operation is
        /// finished. </summary>
        /// <param name="operation"> the operation to manage </param>
        /// <param name="resolvable"> optional parameter, if not null, will be called after finish executing </param>
        /// <exception cref="ArgumentNullException"> <paramref name="operation" /> is null </exception>
        public static ITask<T> AddOperation<T>(T operation) {
            Argument.NotNull(operation);
            _operations.Add(operation);
            var task = new Task<T>();
            EngineHook.StartCoroutine(Wait(operation, task));
            return task;
        }

        public static void AddSynchronousAction(Action action) { WaitingSynchronousActions += action; }

        void Awake() { Flush(); }

        void Start() { Flush(); }

        void Update() {
            Flush();
            //Log.Debug(AssetBundleManager.Manifest.State);
        }

        static void Flush() {
            if (WaitingSynchronousActions == null)
                return;
            WaitingSynchronousActions();
            WaitingSynchronousActions = null;
        }

        static IEnumerator Wait<T>(T operation, IResolvable<T> task) {
            yield return operation;
            _operations.Remove(operation);
            task.Resolve(operation);
        }

    }

}
