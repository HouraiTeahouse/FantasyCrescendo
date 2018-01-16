using System.Threading.Tasks; 
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterName : CharacterUIBase {

  public Text Text;
  public bool UseLongName;
  public bool Uppercase;

  public override Task Initialize(CharacterData character) {
    if (Text != null) {
      var text = UseLongName ? character.LongName : character.ShortName;
      if (Uppercase) {
        text = text.ToUpperInvariant();
      }
      Text.text = text;
    }
    return Task.CompletedTask;
  }

}

}
