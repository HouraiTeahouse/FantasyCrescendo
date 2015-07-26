/*
	Interesting links
	https://gist.github.com/stramit/c98b992c43f7313084ac
	https://gist.github.com/flarb/052467190b84657f10d2
	http://forum.unity3d.com/threads/custom-cursor-how-can-i-simulate-mouse-clicks.268513/
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// Attach this script on your canvas's EventSystem game object
// It glitches the StandaloneInputModule if both are active at the same time

public class CursorInputModule : PointerInputModule  {
	
	// The same event system used on the Canvas
	public EventSystem eventSystem = null;

	// A list of cursor objects inside the canvas
	// It moves only on X and Y axis
	public List<GameObject> cursorObjects ;

	private Vector2 auxVec2;
	private PointerEventData pointer;

	// Use this for initialization
	void Start () {
		base.Start ();
		if( cursorObjects == null || eventSystem == null )
		{
			Debug.LogError( "Set the game objects in the cursor module." );
			GameObject.Destroy( gameObject );
		}
	}
	
	// Process is called once per tick
	public override void Process () 
	{
		// For each player
		for( int i = 0; i < cursorObjects.Count; i++ )
		{
			// Getting objects related to player (i+1)
			GameObject cursorObject = cursorObjects[i]; 
			GetPointerData( i, out pointer, true );

			// Converting the 3D-coords to Screen-coords 
			Vector3 screenPos = Camera.main.WorldToScreenPoint( cursorObject.transform.position );

			auxVec2.x = screenPos.x;
			auxVec2.y = screenPos.y;

			// Raycasting
			pointer.position = auxVec2;
			eventSystem.RaycastAll( pointer, this.m_RaycastResultCache );
			RaycastResult raycastResult = FindFirstRaycast ( this.m_RaycastResultCache );
			pointer.pointerCurrentRaycast = raycastResult;
			this.ProcessMove( pointer );

			pointer.clickCount = 0;
			// Cursor click - adapt for detect input for player (i+1) only
			if( Input.GetButtonDown( "jump" ) )
			{
				pointer.pressPosition = auxVec2;
				pointer.clickTime = Time.unscaledTime;
				pointer.pointerPressRaycast = raycastResult;

				pointer.clickCount = 1;
				pointer.eligibleForClick = true;

				if( this.m_RaycastResultCache.Count > 0 )
				{	
					//Debug.Log ( Time.time + "s Number of objects : "  + rayResults.Count );
					pointer.selectedObject = raycastResult.gameObject;
					pointer.pointerPress = ExecuteEvents.ExecuteHierarchy ( raycastResult.gameObject, pointer, ExecuteEvents.submitHandler );
					pointer.rawPointerPress = raycastResult.gameObject;
				}
				else
				{
					pointer.selectedObject = null;
					pointer.pointerPress = null;
					pointer.rawPointerPress = null;
				}
			} // if( Input.GetButtonDown( "Jump" ) )

			else
			{
				pointer.clickCount = 0;
				pointer.eligibleForClick = false;
				pointer.pointerPress = null;
				pointer.rawPointerPress = null;
			}
		} // for( int i; i < cursorObjects.Count; i++ )
	}
}
