using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype.NetworkLobby
{
    public class DraggablePanel : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            transform.Translate(eventData.delta);
        }
    }
}