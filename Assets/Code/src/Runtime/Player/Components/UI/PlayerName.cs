using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerName : MonoBehaviour, IInitializable<PlayerConfig>, IStateView<PlayerConfig> {

  public Text Text;
  public string Format;

  public Task Initialize(PlayerConfig config) {
    ApplyState(config);
    return Task.CompletedTask;
  }

  public void ApplyState(PlayerConfig config) {
    if (Text == null) return;
    Text.text = string.Format(Format, config.PlayerID + 1);
  }

}
    
}

