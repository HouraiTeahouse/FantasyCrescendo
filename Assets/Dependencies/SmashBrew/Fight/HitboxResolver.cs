using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// The global resolver of hitbox collisions.
    /// </summary>
    public sealed class HitboxResolver : MonoBehaviour {

        /// <summary>
        /// A global list of collisions since the last resolution, sorted by joint priority
        /// </summary>
        private static PriorityList<HitboxCollision> _collisions;

        public struct HitboxCollision {
            public Hitbox Source { get; set; }
            public Hitbox Destination { get; set; }
        }

        [SerializeField, Tooltip("How often to resolve hitbox collsions, in seconds")]
        private float _frequency = 1/60f;

        /// <summary>
        /// The last time hitbox collisions were resolved
        /// </summary>
        private float _timer;

        /// <summary>
        /// Registers a new collision for resoluiton.
        /// </summary>
        /// <param name="src">the source hitbox</param>
        /// <param name="dst">the target hitbox</param>
        public static void AddCollision(Hitbox src, Hitbox dst) {
            // The priority on the collision is the product of the priority on both hitboxes
            _collisions.Add(new HitboxCollision { Destination = dst, Source = src},
                src.Priority * dst.Priority);
        }

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if(_collisions == null)
                _collisions = new PriorityList<HitboxCollision>();
            _timer = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Unity callback. Called repeatedly on a fixed timestep.
        /// </summary>
        void FixedUpdate() {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - _timer;
            if (deltaTime < _frequency)
                return;
            _timer = currentTime - deltaTime % _frequency;
            if (_collisions.Count <= 0)
                return;
            // TODO: actually get this to resolve hitbox collisions instead of just procesesing them
            foreach(HitboxCollision collision in _collisions)
                Hitbox.Resolve(collision.Source, collision.Destination);
            _collisions.Clear();
        }

    }

}
