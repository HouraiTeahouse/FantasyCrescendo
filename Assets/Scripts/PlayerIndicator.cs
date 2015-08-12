using Hourai;
using UnityEngine;

[RequireComponent(typeof (SpriteRenderer))]
public sealed class PlayerIndicator : MonoBehaviour {

    private Vector3 positionBias = new Vector3(0f, 1f, 0f);
    private SpriteRenderer spriteRenderer;
    private Character target;

    public Color Color {
        get { return spriteRenderer ? spriteRenderer.color : Color.clear; }
        set {
            if (spriteRenderer)
                spriteRenderer.color = value;
        }
    }

    public Sprite Sprite {
        get { return spriteRenderer ? spriteRenderer.sprite : null; }
        set {
            if (spriteRenderer)
                spriteRenderer.sprite = value;
        }
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attach(Character targetCharacter) {
        target = targetCharacter;
    }

    private void LateUpdate() {
        bool haveTarget = target != null;
        spriteRenderer.enabled = haveTarget;
        if (haveTarget) {
            Vector3 up = transform.up = target.up;
            transform.position = target.position + up * target.MovementCollider.height + positionBias;
        }
    }

}