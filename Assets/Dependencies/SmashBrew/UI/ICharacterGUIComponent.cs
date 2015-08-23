using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew.UI {

    public interface ICharacterGUIComponent {

        void SetCharacterData(CharacterData data, int pallete, int playerNumber);

    }

}
