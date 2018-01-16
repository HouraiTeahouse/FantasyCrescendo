using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterIcon : CharacterUIBase {

  public Image Image;

  public override async Task Initialize(CharacterData character) {
    if (Image == null) return;
    Image.sprite = await character.Icon.LoadAsync();
  }

}

}
