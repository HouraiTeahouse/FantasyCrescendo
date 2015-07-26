using UnityEngine;
using System.Collections;

public class CharacterMenuInterface : InputInterface {

	public ScreenManager screenManager = null;
	public Animator nextScreenAnimator = null;
	
	// Use this for initialization
	public override void processInputs()
	{
		if( Input.GetButton( "Cancel" ) )
		{
			screenManager.CloseCurrent();
		}
	}
}
