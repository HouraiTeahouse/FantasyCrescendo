using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Players {

public interface IPlayerComponent {

  Task Initialize(PlayerConfig config, bool isView = false);

}

}
