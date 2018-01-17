using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class LoadingScreen : MonoBehaviour {

  public static LoadingScreen Instance { get; private set; }
  static List<Task> Tasks = new List<Task>();
  public Object[] ViewObjects;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;
    UpdateActive();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => UpdateActive();

  void UpdateActive() {
    var isLoading = Tasks.Any(task => !task.IsCompleted);
    foreach (var view in ViewObjects) {
      ObjectUtil.SetActive(view, isLoading);
    }
    if (Tasks.Count > 0) {
      Tasks.RemoveAll(t => t.IsCompleted);
    }
  }

  public static async Task Await(Task task) {
    if (task == null || task.IsCompleted) return;
    Tasks.Add(task);
    if (Instance != null) {
      Instance.UpdateActive();
    }
    await task;
  }

  public static Task AwaitAll() => Task.WhenAll(Tasks);

}

}