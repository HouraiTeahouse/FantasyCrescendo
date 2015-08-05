namespace Crescendo.API {

    public class CharacterDamage : CharacterComponent {

        public float Damage { get; set; }

        protected override void Start() {
            base.Start();
            Character.OnDamage += OnDamage;
        }

        private void OnDestroy() {
            Character.OnDamage -= OnDamage;
        }

        private void OnDamage(float amount) {
            Damage += amount;
        }

    }

}