using UnityEngine;
using System.Collections;

public class Attack : StateMachineBehaviour {

    public enum Type {
        Normal,
        Smash,
        Aerial,
        Special
    }

    public enum Direction {
        Neutral,
        Up,
        Front,
        Back,
        Down
    }

    [SerializeField]
    private Direction direction;

    [SerializeField]
    private Type _type;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

}

public class AttackData : ScriptableObject {

    [SerializeField]
    private Avatar avatar;

}
