using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSlotButton : MonoBehaviour, IPointerDownHandler {

    // The mouse can use OnPointerDown
    public virtual void OnPointerDown(PointerEventData eventData) {
        Debug.Log(" yee " + Time.time + " -  " + eventData.pointerId);
    }

}
