namespace HouraiTeahouse.SmashBrew {

    public abstract class CharacterComponent : BaseBehaviour, IResettable {

        public Character Character { get; set; }

        public Mediator Events {
            get { return Character.Events; }
        }

        public virtual void OnReset() { }

        protected virtual void Start() {
            if (!Character)
                Character = GetComponentInParent<Character>();
        }

    }

}
