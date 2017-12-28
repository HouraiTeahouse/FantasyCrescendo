using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterPhysics : MonoBehaviour, ICharacterSimulation, ICharacterView {

  public CharacterController CharacterController;

  public bool IsGrounded { get; private set; }

  public void Initialize(PlayerConfig config) {
    if (CharacterController == null) {
      CharacterController = GetComponent<CharacterController>();
    }

    var rigidbody = GetComponent<Rigidbody>();
    if (rigidbody == null) {
      rigidbody = gameObject.AddComponent<Rigidbody>();
    }
    rigidbody.useGravity = false;
    rigidbody.isKinematic = true;
    rigidbody.hideFlags = HideFlags.HideInInspector;
  }

  public void Presimulate(PlayerState state) {
    transform.position = state.Position;
    IsGrounded = IsCharacterGrounded(state);
    Debug.Log(IsGrounded);
  }

  public PlayerState Simulate(PlayerState state, PlayerInput input) {
    CharacterController.Move(state.Velocity);
    state.Position = transform.position;
    return state;
  }

  public void ApplyState(PlayerState state) {
    transform.position = state.Position;
    transform.localEulerAngles = (state.Direction ? 0 : 180) * Vector3.up;
  }

  bool IsCharacterGrounded(PlayerState state) {
    if (state.Velocity.y > 0) {
      return false;
    }
    var center = Vector3.zero;
    var radius = 1f;
    if (CharacterController != null) {
        center = CharacterController.center - Vector3.up * (CharacterController.height * 0.50f - CharacterController.radius * 0.5f);
        radius = CharacterController.radius * 0.75f;
    }

    var stageLayers = Config.Get<PhysicsConfig>().StageLayers;

    //TODO(james7132): Figure a good way to handle pass through platforms
    return Physics.OverlapSphere(transform.TransformPoint(center),
                                 radius, stageLayers,
                                 QueryTriggerInteraction.Ignore).Any();
                                 //.Any(col => !_ignoredColliders.Contains(col));
  }


}

}
