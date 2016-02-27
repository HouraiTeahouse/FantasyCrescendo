namespace HouraiTeahouse.SmashBrew.Util {

    public sealed class ModifierGroup<T> {

        public ModifierList<T> In { get; private set; }
        public ModifierList<T> Out { get; private set; }

        public ModifierGroup() {
            In = new ModifierList<T>();
            Out = new ModifierList<T>();
        }

    }
}
