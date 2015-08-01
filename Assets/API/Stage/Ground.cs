using UnityEngine;
using System.Collections;

namespace Crescendo.API
{
    [RequireComponent(typeof(Collider))]
    public class Ground : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            Change(collision.collider, true);
        }

        void OnCollisionExit(Collision collision)
        {
            Change(collision.collider, false);
        }

        void OnTriggerEnter(Collider other)
        {
            Change(other, true);
        }

        void OnTriggerExit(Collider other)
        {
            Change(other, false);
        }

        void Change(Collider target, bool targetValue)
        {
            if (target == null || !target.CompareTag(Game.PlayerTag))
                return;

            Character character = target.GetComponentInParent<Character>();

            character.IsGrounded = targetValue;
        }
    }


}