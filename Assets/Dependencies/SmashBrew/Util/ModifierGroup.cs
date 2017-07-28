namespace HouraiTeahouse.SmashBrew {

    public sealed class ModifierGroup<TSource, TBase> {

        public ModifierGroup() {
            In = new ModifierList<TSource, TBase>();
            Out = new ModifierList<TSource, TBase>();
        }

        public ModifierList<TSource, TBase> In { get; private set; }
        public ModifierList<TSource, TBase> Out { get; private set; }

    }

}