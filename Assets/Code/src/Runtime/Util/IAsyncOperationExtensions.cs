using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {
    
public static class IAsyncOperationExtensions {

  public static AsyncOperationAwaiter<T> GetAwaiter<T>(this AsyncOperationHandle<T> operation) {
    return new AsyncOperationAwaiter<T>(operation);
  }

  public readonly struct AsyncOperationAwaiter<T> : INotifyCompletion {

    readonly AsyncOperationHandle<T> _operation;

    public AsyncOperationAwaiter(AsyncOperationHandle<T> operation) {
      _operation = operation;
    }

    public bool IsCompleted => _operation.Status != AsyncOperationStatus.None;

    public void OnCompleted(Action continuation) => _operation.Completed += (op) => continuation?.Invoke();

    public T GetResult() => _operation.Result;

  }

}

}