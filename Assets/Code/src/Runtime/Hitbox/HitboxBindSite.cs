using HouraiTeahouse.Attributes;
using UnityEngine;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public class HitboxBindSite : MonoBehaviour {

  [SerializeField, ReadOnly] int _id;
  public int Id => _id;

  public Task Initialize(PlayerConfig config, bool isView = false) {
      if (!isView) {
      }
      DestroyImmediate(this);
      return Task.CompletedTask;
  }

  void Reset() => RegenerateID();

  [ContextMenu("Regenerate ID")]
  void RegenerateID() {
    _id = new System.Random().Next();
  }
}

}
