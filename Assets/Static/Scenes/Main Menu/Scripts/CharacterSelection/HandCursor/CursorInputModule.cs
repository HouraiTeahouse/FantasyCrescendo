using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Attach this script on your canvas's EventSystem game object
// It glitches the StandaloneInputModule if both are active at the same time

public class CursorInputModule : PointerInputModule {

    private Vector2 auxVec2;

    // A list of cursor objects inside the canvas
    // It moves only on X and Y axis
    public List<GameObject> cursorObjects;

    // The same event system used on the Canvas
    public EventSystem eventSystem = null;
    private PointerEventData pointer;

    // Use this for initialization
    protected override void Start() {
        base.Start();
        if (cursorObjects == null || eventSystem == null) {
            Debug.LogError("Set the game objects in the cursor module.");
            Destroy(gameObject);
        }
    }

    // Process is called once per tick
    public override void Process() {
        // For each player
        for (var i = 0; i < cursorObjects.Count; i++) {
            // Getting objects related to player (i+1)
            GameObject cursorObject = cursorObjects[i];
            GetPointerData(i, out pointer, true);

            // Converting the 3D-coords to Screen-coords 
            Vector3 screenPos = Camera.main.WorldToScreenPoint(cursorObject.transform.position);

            auxVec2.x = screenPos.x;
            auxVec2.y = screenPos.y;

            // Raycasting
            pointer.position = auxVec2;
            eventSystem.RaycastAll(pointer, m_RaycastResultCache);
            RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);
            pointer.pointerCurrentRaycast = raycastResult;
            ProcessMove(pointer);

            pointer.clickCount = 0;

            // Cursor click - adapt for detect input for player (i+1) only
            if (Input.GetButtonDown("Jump")) {
                pointer.pressPosition = auxVec2;
                pointer.clickTime = Time.unscaledTime;
                pointer.pointerPressRaycast = raycastResult;

                pointer.clickCount = 1;
                pointer.eligibleForClick = true;

                if (m_RaycastResultCache.Count > 0) {
                    //Debug.Log ( Time.time + "s Number of objects : "  + rayResults.Count );
                    pointer.selectedObject = raycastResult.gameObject;
                    pointer.pointerPress = ExecuteEvents.ExecuteHierarchy(raycastResult.gameObject,
                                                                          pointer,
                                                                          ExecuteEvents.submitHandler);
                    pointer.rawPointerPress = raycastResult.gameObject;
                } else {
                    pointer.selectedObject = null;
                    pointer.pointerPress = null;
                    pointer.rawPointerPress = null;
                }
            } // if( Input.GetButtonDown( "Jump" ) )
            else {
                pointer.clickCount = 0;
                pointer.eligibleForClick = false;
                pointer.pointerPress = null;
                pointer.rawPointerPress = null;
            }
        } // for( int i; i < cursorObjects.Count; i++ )
    }

}