namespace HouraiTeahouse.SmashBrew {

    public class PlayerJumpEvent {

        public bool ground;
        public int remainingJumps;

    }

    public class PlayerGroundEvent {

        public bool Grounded;

    }
        
    public class PlayerDieEvent {

        public bool Revived;
        public Player Player;

    }

    public class PlayerDamageEvent {

        public float damage;
        public float currentDamage;

    }

    public class PlayerHealEvent {

        public float healing;
        public float currentDamage;

    }

}
