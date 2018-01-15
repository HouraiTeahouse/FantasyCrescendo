using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {
    
public interface IViewFactory<TState, TConfig> {
  Task<IStateView<TState>[]> CreateViews(TConfig config);
}

}