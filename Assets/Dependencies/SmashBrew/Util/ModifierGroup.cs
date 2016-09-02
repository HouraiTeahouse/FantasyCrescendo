namespace HouraiTeahouse.SmashBrew {

    public sealed class ModifierGroup<T> {

        public ModifierGroup() {
            In = new ModifierList<T>();
            Out = new ModifierList<T>();
        }

        public ModifierList<T> In { get; private set; }
        public ModifierList<T> Out { get; private set; }

    }

}