using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IPlayerComponent {

  Task Initialize(PlayerConfig config, bool isView = false);

}

}
