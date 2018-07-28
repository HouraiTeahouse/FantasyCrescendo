using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerName : MonoBehaviour, IInitializable<PlayerConfig>, IStateView<PlayerConfig> {

  public TMP_Text Text;
  public string Format;

  public Task Initialize(PlayerConfig config) {
    ApplyState(ref config);
    return Task.CompletedTask;
  }

  public void ApplyState(ref PlayerConfig config) {
    if (Text == null) return;
    Text.text = string.Format(Format, config.PlayerID + 1);
  }

}
    
}

