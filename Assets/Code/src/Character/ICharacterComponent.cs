using HouraiTeahouse.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface ICharacterComponent {

  ITask Initialize(PlayerConfig config, bool isView = false);

}

}
