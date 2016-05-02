
namespace HouraiTeahouse.SmashBrew.UI {

    public class SceneSelector : SceneUIComponent, IPlayerClickable{
        public void Click(Player player) {
            if (Scene) {
                Scene.Load ();
            }
        }
    }
}
