namespace HouraiTeahouse.SmashBrew {
    public class JumpEvent {

        public bool ground;
        public int remainingJumps;

    }

    public class GroundEvent {

        public bool Grounded;

    }
        
    public class PlayerDieEvent {

        public bool Revived;
        public Player Player;

    }
}
