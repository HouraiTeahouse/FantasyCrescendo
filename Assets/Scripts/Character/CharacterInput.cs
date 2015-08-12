using UnityEngine;

namespace Hourai {

    public interface ICharacterInput {

        Vector2 Movement { get; }
        bool Jump { get; }
        bool Crouch { get; }
        bool Attack { get; }
        bool Special { get; }
        bool Shield { get; }

    }

}