
namespace Crescendo.API
{
    
    public class CharacterDamage : CharacterComponent
    {

        public float Damage { get; set; }

        protected override void Start()
        {
            base.Start();
            Character.OnDamage += OnDamage;
        }

        void OnDestroy()
        {
            Character.OnDamage -= OnDamage;
        }

        void OnDamage(float amount)
        {
            Damage += amount;
        }

    }


}