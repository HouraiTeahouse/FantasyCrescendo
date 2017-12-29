using HouraiTeahouse.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface IInitializable<T> {

  ITask Initialize(T config);

}

}
