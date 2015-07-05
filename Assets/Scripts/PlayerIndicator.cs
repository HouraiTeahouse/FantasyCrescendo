using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerIndicator : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Character target;
    private Transform targetTransform;

    public Color Color {
        get {
            return spriteRenderer != null ? spriteRenderer.color : Color.clear;
        }
        set {
            if (spriteRenderer != null)
                spriteRenderer.color = value;
        }
    }

    public Sprite Sprite {
        get {
            return spriteRenderer != null ? spriteRenderer.sprite : null;
        }
        set {
            if (spriteRenderer != null)
                spriteRenderer.sprite = value;
        }
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attach(Character targetCharacter) {
        target = targetCharacter;
        targetTransform = targetCharacter.transform;
    }

    void LateUpdate() {
        bool haveTarget = target != null;
        spriteRenderer.enabled = haveTarget;
        if (haveTarget)
            transform.position = targetTransform.position + targetTransform.up*target.Height;
    }

}
