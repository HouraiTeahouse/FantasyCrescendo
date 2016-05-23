namespace HouraiTeahouse.SmashBrew.UI {

    public abstract class PlayerClickableUI : PlayerUIComponent, IPlayerClickable {

        protected bool CheckIdenticalPlayer { get; set; }

        public void Click(Player player) {
            if (PlayerCheck(player))
                return;
            OnPlayerClick(player);
        }

        protected virtual bool PlayerCheck(Player player) {
            return player == null || (CheckIdenticalPlayer && player != Player);
        }

        protected abstract void OnPlayerClick(Player player);
    }

    public class PlayerCycleType : PlayerClickableUI {

        protected override void OnPlayerClick(Player player) {
            player.CycleType();
        }
    }

}
