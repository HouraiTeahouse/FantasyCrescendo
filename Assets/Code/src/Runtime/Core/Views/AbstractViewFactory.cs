using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

// An abstract IViewFactory implementation that derives from MonoBehavior
// Mainly to search for via FindObjectsOfType
public abstract class AbstractViewFactory<TState, TConfig> : MonoBehaviour, IViewFactory<TState, TConfig> {
  public abstract Task<IStateView<TState>[]> CreateViews(TConfig config);
}

}