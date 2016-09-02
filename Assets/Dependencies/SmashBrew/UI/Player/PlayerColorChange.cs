namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerColorChange : PlayerClickableUI {

        protected override bool PlayerCheck(Player player) {
            return player == null || !(player.Type.IsCPU || player == Player);
        }

        protected override void OnPlayerClick(Player player) { player.Selection.Pallete++; }

    }

}