using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInputManager : MonoBehaviour {

	public ScreenManager screenManager = null;
	public List<GameObject> availableScreens = null;
	
	private List<Animator> animators = null;
	private List<InputInterface> inputInterfaces = null;

	// Use this for initialization
	void Start () {
		animators = new List<Animator>();
		inputInterfaces = new List<InputInterface>();
		foreach( GameObject go in availableScreens )
		{
			Animator anim = go.GetComponent<Animator>();
			InputInterface inputInterface = go.GetComponent<InputInterface>();

			if( anim == null )
			{
				Debug.LogError( "The " + go.name + " must have an Animator component." );
				GameObject.Destroy( this.gameObject );
				return;
			}
			if( inputInterface == null )
			{
				Debug.LogError( "The " + go.name + " must have an InputInterface component." );
				GameObject.Destroy( this.gameObject );
				return;
			}

			animators.Add( anim );
			inputInterfaces.Add( inputInterface );
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animator openedAnimator = screenManager.getOpenedAnimator();
		if( openedAnimator == null )
		{
			return;
		}

		int i = 0;
		for( i = 0; i < animators.Count; i++ )
		{
			if( animators[i] == openedAnimator )
			{
				inputInterfaces[i].processInputs();
				return;
			}
		}

		Debug.LogError( "The current animator has no available inputInterface" );
		GameObject.Destroy( this.gameObject );
	
	}
}
