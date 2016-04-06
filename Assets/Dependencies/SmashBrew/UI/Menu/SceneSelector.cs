namespace HouraiTeahouse.SmashBrew.UI {

    public class StageSelector : CharacterUIComponent, IPlayerClickable {
        public void Click(Player player) {
            if(Character && Character.IsSelectable)
                player.SelectedCharacter = Character;
        }
    }
}
