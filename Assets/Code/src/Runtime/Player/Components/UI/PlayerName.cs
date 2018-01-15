using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerName : MonoBehaviour, IInitializable<PlayerConfig> {

  public Text Text;
  public string Format;

  public Task Initialize(PlayerConfig config) {
    if (Text != null) {
      Text.text = string.Format(Format, config.PlayerID + 1);
    }
    return Task.CompletedTask;
  }

}
    
}

