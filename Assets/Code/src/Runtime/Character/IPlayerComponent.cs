using HouraiTeahouse.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IPlayerComponent {

  ITask Initialize(PlayerConfig config, bool isView = false);

}

}
