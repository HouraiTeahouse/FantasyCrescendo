namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerCharacterChange : CharacterUIComponent, IPlayerClickable {

        public void Click(Player player) {
            if (Character && Character.IsSelectable)
                player.Selection.Character = Character;
        }

    }

}